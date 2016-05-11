using System;
using System.Diagnostics;

namespace CSharpCodeGenerator
{
    public static class Log
    {
        public static bool WriteToConsole = true;
        public static bool LogClassName = true;

        public static void Write (string text)
        {
            var output = $"{text}";
            if (LogClassName) {
                var callerName = new StackFrame (1, true).GetMethod ().Name;
                output = $"({ callerName}) {output}";
            }

            if (WriteToConsole) {
                Console.WriteLine (output);
            }
        }
    }
}

