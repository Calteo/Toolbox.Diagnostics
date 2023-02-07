namespace Toolbox.Diagnostics
{
    /// <summary>
    /// Represens a capture of an object or property.
    /// </summary>
    public class TraceCapture
    {
        public int Id { get; set; } = 0;
        public bool Referenced { get; set; }
        public string Name { get; set; } = "";
        public string Text { get; set; } = "";
        public string TextAndId
        {
            get 
            {                 
                return Text + (Referenced ? $" - Id<{Id}>" : ""); 
            }
        }

        public TraceCapture[] Children { get; set; } = Array.Empty<TraceCapture>();

    }
}
