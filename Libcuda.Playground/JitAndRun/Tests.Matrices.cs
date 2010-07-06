﻿using System;
using XenoGears.Assertions;
using XenoGears.Functional;
using XenoGears.Logging;

namespace Libcuda.Playground.JitAndRun
{
    public abstract partial class Tests
    {
        private float[,] RandMatrix(int height, int width)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            Func<int> rand = () => rng.Next() % 3;
//            Func<int> rand = () => 1;

            var matrix = new float[height, width];
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    matrix[i, j] = rand();
                }
            }

            return matrix;
        }

        private void PrintMatrix(String headline, float[,] m)
        {
            Log.Trace(headline + Environment.NewLine + m.StringJoin() + Environment.NewLine);
        }

        private void AssertAreTheSame(float[,] a, float[,] b)
        {
            var haveSameDims = a.Height() == b.Height() && a.Width() == b.Width();
            var areTheSame = haveSameDims ? ((Func<bool>)(() =>
            {
                for (var i = 0; i < a.Height(); i++)
                {
                    for (var j = 0; j < a.Width(); j++)
                    {
                        if (a[i, j] != b[i, j]) return false;
                    }
                }

                return true;
            }))() : false;
            if (!areTheSame)
            {
                Log.TraceLine("*".Repeat(120));
                Log.TraceLine("ERROR! Calculated matrix ain't equal to reference result.");
                Log.TraceLine();

                PrintMatrix("Expected: ", a);
                Log.TraceLine();
                PrintMatrix("Actual: ", b);
                AssertionHelper.Fail();
            }
        }

        private float[,] ReferenceMul(float[,] a, float[,] b)
        {
            var dim = a.Width().AssertThat(_ => a.Width() == b.Height());
            int a_height = a.Height(), b_width = b.Width();
            var c = new float[a_height, b_width];

            for (var i = 0; i < a_height; ++i)
            {
                for (var j = 0; j < b_width; ++j)
                {
                    var c_value = 0f;
                    for (var k = 0; k < dim; ++k)
                    {
                        c_value += a[i, k] * b[k, j];
                    }

                    c[i, j] = c_value;
                }
            }

            return c;
        }
    }
}
