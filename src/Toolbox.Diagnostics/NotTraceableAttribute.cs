using System;

namespace Toolbox.Diagnostics
{
    /// <summary>
    /// Prevents an element to appear in the trace output.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotTraceableAttribute : Attribute
    {
    }
}
