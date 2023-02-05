namespace Toolbox.Diagnostics
{
    /// <summary>
    /// Listener that formats objects to a <see cref="TextWriter"/>.
    /// </summary>
    public class ObjectTraceTextWriterListener : ObjectTraceTextListener
    {
        /// <inheritdoc />
        public ObjectTraceTextWriterListener(TextWriter writer) : this(null, writer)
        {
        }

        /// <inheritdoc />
        public ObjectTraceTextWriterListener(string? name, TextWriter writer)
            : base(name)
        {
            Writer = writer;
            Template = Properties.Resources.ObjectTraceTextListenerTemplate;
        }

        public TextWriter Writer { get; }

        protected override void AppendLine(string text)
        {
            Writer.WriteLine(text);
        }
    }
}
