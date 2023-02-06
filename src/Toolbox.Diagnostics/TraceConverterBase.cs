using System.Reflection;

namespace Toolbox.Diagnostics
{
    public abstract class TraceConverterBase
    {
        public TraceConverterBase(ObjectTraceListener listener)
        {
            Listener = listener;
        }

        protected internal abstract TraceCapture CaptureCore(object obj);
        protected internal abstract Type ConvertType { get; }

        protected internal ObjectTraceListener Listener { get; internal set; }
        
        protected TraceCapture[] GetChildren(object obj)
        {
            var properties = obj.GetType()
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                .Where(p => p.GetCustomAttribute<NotTraceableAttribute>(true) == null);

            return properties.Select(p => Capture(obj, p)).ToArray();
        }

        private TraceCapture Capture(object obj, PropertyInfo property)
        {
            try
            {
                var value = property.GetValue(obj);

                var capture = value != null
                    ? (GetConverter(property) ?? Listener.GetConverter(value)).CaptureCore(value)
                    : new TraceCapture { Text = "<null>" };

                capture.Name = property.Name;
                return capture;
            }
            catch (Exception exception)
            {
                return new TraceCapture { Name = property.Name, Text = exception.Message };
            }                        
        }

        private TraceConverterBase? GetConverter(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<TraceConverterAttribute>(true);
            if (attribute == null) return null;

            var args = new object[attribute.Arguments.Length+1];
            attribute.Arguments.CopyTo(args, 1);
            args[0] = Listener;

            return (TraceConverterBase?)Activator.CreateInstance(attribute.ConvertType, args);
        }
    }
}
