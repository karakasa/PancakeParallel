using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelCore.Interface
{
    public interface IScheduler
    {
        /// <summary>
        /// Run tasks. Return after tasks are completed or the operation is aborted.
        /// Please notice the tasks are not guaranteed to run asynchronously.
        /// </summary>
        void Run();
        /// <summary>
        /// Run tasks synchronously.
        /// </summary>
        void RunSynchronously();
    }

    public interface IScheduler<T> : IScheduler, IExceptionStorage<T>
    {

    }
}
