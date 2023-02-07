namespace Toolbox.Diagnostics
{
    class TraceConverterString : TraceConverter<string>
    {
        public TraceConverterString(ObjectTraceListener listener) : base(listener)
        {
        }

        protected override TraceCapture Capture(string obj, Dictionary<object, TraceCapture> captured)
        {
            return new TraceCapture { Text = $"'{obj}'" };
        }
    }
}
