using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Diagnostics.App
{
    internal class Node
    {
        public Node? Parent { get; }
        public List<Node> Children { get; } = new List<Node>();

        public Node(Node? parent = default(Node))
        {
            Parent = parent;
        }
    }
}
