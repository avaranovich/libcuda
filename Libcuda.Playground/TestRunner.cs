﻿using System;
using System.Collections.Generic;

namespace Libcuda.Playground
{
    internal class TestRunner
    {
        public static void Main(String[] args)
        {
            // see more details at http://www.nunit.org/index.php?p=consoleCommandLine&r=2.5.5
            var nunitArgs = new List<String>();
//            nunitArgs.Add("/run:Libcuda.Playground.DevicesAndVersions");
//            nunitArgs.Add("/run:Libcuda.Playground.JitAndRun.MatMul");
            nunitArgs.Add("/run:Libcuda.Playground.JitAndRun.MatMul_Fast");
            nunitArgs.Add("/include:Hot");
            nunitArgs.Add("/domain:None");
            nunitArgs.Add("/noshadow");
            nunitArgs.Add("/nologo");
            nunitArgs.Add("Libcuda.Playground.exe");
            NUnit.ConsoleRunner.Runner.Main(nunitArgs.ToArray());
        }
    }
}
