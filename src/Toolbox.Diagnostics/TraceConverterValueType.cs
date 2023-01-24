using System;

namespace Toolbox.Diagnostics
{
    public class TraceConverterValueType : TraceConverter<ValueType>
    {
        public TraceConverterValueType(ObjectTraceListener listener)
            : base(listener)
        {
            Format = "";
        }

        public TraceConverterValueType(ObjectTraceListener listener, string format) 
            : base(listener)
        {
            Format = format;
        }

        public string Format { get; }

        protected override TraceCapture Capture(ValueType obj)
        {
            var text = string.Format($"{{0:{Format}}}", obj);
            return new TraceCapture { Text = text };
        }
    }
}
