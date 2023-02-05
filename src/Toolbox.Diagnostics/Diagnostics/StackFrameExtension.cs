using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Diagnostics.Diagnostics
{
    internal static class StackFrameExtension
    {
       public static string ToMethodString(this StackFrame frame, bool includeSource = true)
        {
            var builder = new StringBuilder();

            var methodBase = frame.GetMethod();
            if (methodBase is MethodInfo methodInfo)
            {
                builder.Append(methodInfo.ReturnType.Name);
                builder.Append(' ');

                var declaringType = methodBase.DeclaringType;
                if (declaringType != null)
                {
                    builder.Append(declaringType.FullName);
                    builder.Append('.');
                }
                builder.Append(methodBase.Name);
                if (methodBase.IsGenericMethod)
                {
                    builder.Append('[');
                    builder.Append(string.Join(",", methodBase.GetGenericArguments().Select(t => t.Name)));
                    builder.Append(']');
                }
                builder.Append('(');
                builder.Append(string.Join(", ", methodBase.GetParameters().Select(p => p.ToString())));
                builder.Append(')');
                if (frame.HasSource() && includeSource)
                {
                    builder.Append($" - {frame.GetFileName()}:{frame.GetFileLineNumber()}");
                }
            }
            else if (methodBase != null)
            {
                builder.Append(methodBase.ToString());
            }
            else
            {
                builder.Append(frame.ToString());
            }

            return builder.ToString();
        }
    }
}
