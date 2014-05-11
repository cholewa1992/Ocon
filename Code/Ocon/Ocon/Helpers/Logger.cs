using System;
using System.IO;

namespace Ocon.Helper
{
    internal static class Logger
    {
        internal static void Write(TextWriter target, string msg)
        {
            if (target != null) target.WriteLineAsync(DateTime.Now + " | " + msg);
        }
    }
}