using System.Security;

namespace Toolbox.Diagnostics
{
    class TraceConverterSecureString : TraceConverter<SecureString>
    {
        public TraceConverterSecureString(ObjectTraceListener listener) : base(listener)
        {
        }

        protected override TraceCapture Capture(SecureString obj)
        {
            return new TraceCapture { Text = $"'***'" };
        }
    }
}
