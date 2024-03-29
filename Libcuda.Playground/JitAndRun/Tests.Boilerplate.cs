﻿using System;
using System.Text.RegularExpressions;
using Libcuda.Tracing;
using NUnit.Framework;

namespace Libcuda.Playground.JitAndRun
{
    public abstract partial class Tests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            MultiplexLogs(Traces.Init, Traces.Jit, Traces.Run);
        }

        protected override String PreprocessResult(String s_actual)
        {
            s_actual = Regex.Replace(s_actual, @"cuModuleLoadDataEx_[a-fA-f0-9]*", "cuModuleLoadDataEx");
            s_actual = Regex.Replace(s_actual, @"gpu='[\d\w]*?'", "gpu='<HardwareIsa>'");
            s_actual = Regex.Replace(s_actual, @"\d{2}:\d{2}:\d{2}\.\d{6}", "<ElapsedTime>");
            s_actual = Regex.Replace(s_actual, @"0x[a-fA-f0-9]{8}", "<Handle>");
            s_actual = Regex.Replace(s_actual, @"\.entry\s+(?<name>\w+).*?\{.*?\}", ".entry ${name} {...}", RegexOptions.Singleline);
            return s_actual;
        }
    }
}
