using System;
using System.Text.RegularExpressions;

namespace Libcuda.Playground.JitAndRun
{
    public abstract partial class Tests
    {
        protected override String PreprocessResult(String s_actual)
        {
            s_actual = Regex.Replace(s_actual, @"\d{2}:\d{2}:\d{2}\.\d{6}", "<TimeSpan>");
            return Regex.Replace(s_actual, @"0x[a-fA-f0-9]{8}", "<Handle>");
        }
    }
}
