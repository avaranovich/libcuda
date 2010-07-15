using System;
using XenoGears.Threading;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        private static WorkerThread _worker = new WorkerThread{Name = "Libcuda worker thread"};
        private static int _worker_nativeThreadId;

        private static void MarshalToWorkerThread(Action task)
        {
            if (NativeThread.Id == _worker_nativeThreadId)
            {
                task();
            }
            else
            {
                _worker.Invoke(() =>
                {
                    if (_worker_nativeThreadId == 0)
                    {
                        using (NativeThread.Affinitize(out _worker_nativeThreadId))
                        {
                            task();
                        }
                    }
                    else
                    {
                        using (NativeThread.Affinitize(_worker_nativeThreadId))
                        {
                            task();
                        }
                    }
                });
            }
        }

        private static T MarshalToWorkerThread<T>(Func<T> task)
        {
            if (NativeThread.Id == _worker_nativeThreadId)
            {
                return task();
            }
            else
            {
                return _worker.Invoke(() =>
                {
                    if (_worker_nativeThreadId == 0)
                    {
                        using (NativeThread.Affinitize(out _worker_nativeThreadId))
                        {
                            return task();
                        }
                    }
                    else
                    {
                        using (NativeThread.Affinitize(_worker_nativeThreadId))
                        {
                            return task();
                        }
                    }
                });
            }
        }
    }
}
