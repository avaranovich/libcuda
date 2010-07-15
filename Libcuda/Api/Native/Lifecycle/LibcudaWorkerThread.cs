using System;
using System.Diagnostics;
using Libcuda.Exceptions;
using XenoGears.Threading;

namespace Libcuda.Api.Native
{
    public static partial class nvcuda
    {
        [WorkerThread(Name = "Libcuda worker thread", IsAffined = true, IsBackground = true)]
        [DebuggerNonUserCode]
        internal class LibcudaWorkerThread : WorkerThread
        {
            protected override void Initialize()
            {
                InitializeDriver();
                InitializeGlobalContext();
            }

            protected override Func<T> Wrap<T>(Func<T> task)
            {
                return () =>
                {
                    try
                    {
                        return task();
                    }
                    catch (CudaException)
                    {
                        throw;
                    }
                    catch (DllNotFoundException dnfe)
                    {
                        throw new CudaException(CudaError.NoDriver, dnfe);
                    }
                    catch (Exception e)
                    {
                        throw new CudaException(CudaError.Unknown, e);
                    }
                };
            }

            protected override Action Wrap(Action task)
            {
                return () =>
                {
                    try
                    {
                        task();
                    }
                    catch (CudaException)
                    {
                        throw;
                    }
                    catch (DllNotFoundException dnfe)
                    {
                        throw new CudaException(CudaError.NoDriver, dnfe);
                    }
                    catch (Exception e)
                    {
                        throw new CudaException(CudaError.Unknown, e);
                    }
                };
            }
        }
    }
}