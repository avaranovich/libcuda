﻿using System;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        private static LibcudaWorkerThread _worker = new LibcudaWorkerThread();

        private static void Wrap(Action task)
        {
            _worker.Invoke(task);
        }

        private static T Wrap<T>(Func<T> task)
        {
            return _worker.Invoke(task);
        }
    }
}
