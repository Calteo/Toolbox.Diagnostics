namespace Toolbox.Diagnostics
{
    public class TraceConverterObject : TraceConverter<object>
    {
        public TraceConverterObject(ObjectTraceListener listener) : base(listener)
        {
        }

        protected override TraceCapture Capture(object obj, Dictionary<object, TraceCapture> captured)
        {
            if (captured.ContainsKey(obj))
            {
                captured[obj].Referenced = true;
                return new TraceCapture { Text = $"Ref<{captured[obj].Id}>" };
            }
            else
            {
                var capture = new TraceCapture
                {
                    Id = captured.Count+1,
                    Text = obj.GetType().FullName ?? obj.GetType().ToString()                    
                };
                
                captured.Add(obj, capture);
                capture.Children = GetChildren(obj, captured);

                return capture;
            }
        }
    }
}
