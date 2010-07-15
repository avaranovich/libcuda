using System;
using System.Text.RegularExpressions;

namespace Libcuda.Playground.JitAndRun
{
    public abstract partial class Tests
    {
        protected override String PreprocessResult(String s_actual)
        {
            s_actual = Regex.Replace(s_actual, @"cuModuleLoadDataEx_[a-fA-f0-9]*", "cuModuleLoadDataEx");
            s_actual = Regex.Replace(s_actual, @"\d{2}:\d{2}:\d{2}\.\d{6}", "<TimeSpan>");
            s_actual = Regex.Replace(s_actual, @"0x[a-fA-f0-9]{8}", "<Handle>");
            s_actual = Regex.Replace(s_actual, @".entry\s*(?<name>\w*?)\s*\(.*?\)\s*\{.*?\}", ".entry ${name} {...}", RegexOptions.Singleline);
            return s_actual;
        }
    }
}
