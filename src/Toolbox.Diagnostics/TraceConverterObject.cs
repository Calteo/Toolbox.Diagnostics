namespace Toolbox.Diagnostics
{
    public class TraceConverterObject : TraceConverter<object>
    {
        public TraceConverterObject(ObjectTraceListener listener) : base(listener)
        {
        }

        protected override TraceCapture Capture(object obj)
        {
            return new TraceCapture 
            {
                Text = obj.GetType().FullName ?? obj.GetType().ToString(),
                Children = GetChildren(obj)
            };
        }
    }
}
