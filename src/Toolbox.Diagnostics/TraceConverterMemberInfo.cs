using System.Reflection;

namespace Toolbox.Diagnostics
{
    internal class TraceConverterMemberInfo : TraceConverter<MemberInfo>
    {
        public TraceConverterMemberInfo(ObjectTraceListener listener) : base(listener)
        {
        }

        protected override TraceCapture Capture(MemberInfo obj, Dictionary<object, TraceCapture> captured)
        {
            return new TraceCapture
            {
                Text = $"[{obj.DeclaringType?.Namespace}] - {obj}"
            };
        }
    }
}
