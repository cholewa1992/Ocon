using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextawareFramework.Helper
{
    internal static class Logger
    {
        internal static void Write(TextWriter target, string msg)
        {
            if (target != null) target.WriteLineAsync(msg);
        }
    }
}
