using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp
{
    public class TraceHelper
    {
        public static void TraceInformation(string str, params object[] args)
        {
            TraceInformation(ConsoleColor.Yellow, str, args);
        }

        public static void TraceInformation(ConsoleColor color, string str, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Trace.TraceInformation(str, args);
            Console.ForegroundColor = oldColor;
        }

        public static void TraceError(string str, params object[] args)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Trace.TraceError(str, args);
            Console.ForegroundColor = oldColor;
        }
    }
}
