using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Toolbox.Diagnostics
{
    class TraceConverterEnumerable : TraceConverter<IEnumerable>
    {
        public TraceConverterEnumerable(ObjectTraceListener listener) : base(listener)
        {
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsArray)
                return $"{type.Namespace}.{type.GetElementType()?.Name ?? typeof(Array).Name}[]";

            if (!type.IsGenericType)
                return $"{type.Namespace}.{type.Name}";

            var name = type.GetGenericTypeDefinition().Name;
            var index = name.IndexOf('`');
            name = index == -1 ? name : name.Substring(0, index);

            return $"{type.Namespace}.{name}<{string.Join(",", type.GetGenericArguments().Select(GetTypeName))}>";
        }

        protected override TraceCapture Capture(IEnumerable obj, Dictionary<object, TraceCapture> captured)
        {
            var capture = new TraceCapture { Text = $"{GetTypeName(obj.GetType())}" };
            var children = new List<TraceCapture>();
            var enumarator = obj.GetEnumerator();
            var index = 0;
            while (enumarator.MoveNext())
            {
                var childCapture = Listener.GetConverter(enumarator.Current).CaptureCore(enumarator.Current, captured);
                childCapture.Name = $"[{index++}]";
                children.Add(childCapture);
                if (index >= Listener.MaxCollectionCount)
                {
                    var start = index;
                    while (enumarator.MoveNext()) index++;
                    children.Add(new TraceCapture { Name = $"[{start}-{index - 1}]", Text = "..." });
                    break;
                }
            }
            capture.Children = children.ToArray();
            capture.Text += $" - {index} elements";            

            return capture;
        }
    }
}
