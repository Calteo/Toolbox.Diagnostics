namespace Toolbox.Diagnostics
{
    /// <summary>
    /// Represens a capture of an object or property.
    /// </summary>
    public class TraceCapture
    {
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
        public TraceCapture[] Children { get; set; } = Array.Empty<TraceCapture>();

    }
}
