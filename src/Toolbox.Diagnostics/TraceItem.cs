using System.Diagnostics;
using System.Reflection;

namespace Toolbox.Diagnostics
{
    public class TraceItem
    {
        public TraceItem(string source, TraceEventType eventType, int id, int threadId, int processId, string text, StackFrame caller, TraceCapture[] objects)
        {
            Source = source;
            EventType = eventType;
            Id = id;
            ThreadId = threadId;
            ProcessId = processId;
            Text = text;
            Caller = caller;
            Objects = objects;
        }

        public DateTime Timestamp { get; } = DateTime.Now;
        public string Source { get; }
        public TraceEventType EventType { get; }
        public int Id { get; }
        public int ThreadId { get; }
        public int ProcessId { get; }
        public string Text { get; }
        public StackFrame Caller { get; }

        public MethodBase? Method => Caller.GetMethod();
        public string MethodSignature
        {
            get
            {
                var method = Method;
                var returnType = "";
                if (method is MethodInfo methodInfo)
                    returnType = $"{methodInfo.ReturnType.Name} ";

                return $"{returnType}{method?.Name ?? Caller.ToString()}()";
            }
        }
        public TraceCapture[] Objects { get; internal set; }
    }
}
