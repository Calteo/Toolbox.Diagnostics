using System.Diagnostics;
using System.Reflection;
using Toolbox.Diagnostics.Diagnostics;

namespace Toolbox.Diagnostics
{
    public class TraceItem
    {
        public TraceItem(string source, TraceEventType eventType, int id, int threadId, int processId, string text, StackFrame[] frames, TraceCapture[] objects)
        {
            Source = source;
            EventType = eventType;
            Id = id;
            ThreadId = threadId;
            ProcessId = processId;
            Text = text;
            Frames = frames;
            Objects = objects;
        }

        public DateTime Timestamp { get; } = DateTime.Now;
        public string Source { get; }
        public TraceEventType EventType { get; }
        public int Id { get; }
        public int ThreadId { get; }
        public int ProcessId { get; }
        public string Text { get; }
        public StackFrame Caller => Frames[0];
        public StackFrame[] Frames { get; }
        public MethodBase? Method => Caller.GetMethod();
        public string MethodSignature => Caller.ToMethodString(false);
        public string MethodName => Method?.Name ?? Caller.ToString();
        public string MethodReturnType => (Method is MethodInfo methodInfo) ? methodInfo.ReturnType.Name : "<no return type>";
        public string ClassName => Method?.DeclaringType?.Name ?? "<no class>";
        public string ClassFullName => Method?.DeclaringType?.FullName ?? "<no class>";

        public TraceCapture[] Objects { get; internal set; }
    }
}
