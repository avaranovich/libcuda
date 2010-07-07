using System;
using System.Diagnostics;
using Libcuda.Tracing;

namespace Libcuda.Api.Jit
{
    [DebuggerNonUserCode]
    internal static class Log
    {
        public static void Write(Object o)
        {
            Traces.Jit.Write(o);
        }

        public static void Write(String message)
        {
            Traces.Jit.Write(message);
        }

        public static void Write(String message, params Object[] args)
        {
            Traces.Jit.Write(String.Format(message, args));
        }

        public static void WriteLine(Object o)
        {
            Write(o);
            WriteLine();
        }

        public static void WriteLine(String message)
        {
            Write(message);
            WriteLine();
        }

        public static void WriteLine(String message, params Object[] args)
        {
            Write(message, args);
            WriteLine();
        }

        public static void WriteLine()
        {
            Write(Environment.NewLine);
        }
    }
}