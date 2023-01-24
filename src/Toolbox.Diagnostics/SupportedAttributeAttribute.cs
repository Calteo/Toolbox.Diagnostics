using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Diagnostics
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    class SupportedAttributeAttribute : Attribute
    {
        public SupportedAttributeAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
