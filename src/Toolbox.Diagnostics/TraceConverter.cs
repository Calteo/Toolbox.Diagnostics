namespace Toolbox.Diagnostics
{
    public abstract class TraceConverter<T> : TraceConverterBase
    {
        public TraceConverter(ObjectTraceListener listener) : base(listener)
        {
        }

        protected internal override Type ConvertType => typeof(T);

        protected abstract TraceCapture Capture(T obj, Dictionary<object, TraceCapture> captured);

        protected internal override TraceCapture CaptureCore(object obj, Dictionary<object, TraceCapture> captured)
        {
            return Capture((T)obj, captured);
        }
    }
}
