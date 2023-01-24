namespace Toolbox.Diagnostics
{
    class TraceConverterString : TraceConverter<string>
    {
        public TraceConverterString(ObjectTraceListener listener) : base(listener)
        {
        }

        protected override TraceCapture Capture(string obj)
        {
            return new TraceCapture { Text = $"'{obj}'" };
        }
    }
}
