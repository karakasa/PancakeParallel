using ParallelCore.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelCore.Interface
{
    public interface ITaskProvider<T>
    {
        /// <summary>
        /// Get the next available task. This method is guaranteed to run on the main scheduler thread.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        TaskProviderState TryGetNextAvailableTask(out T identifier);

        /// <summary>
        /// Run a specific task. This method is NOT guaranteed to run on the main scheduler thread.
        /// </summary>
        /// <param name="identifier"></param>
        void RunTaskSynchronously(T identifier);

        /// <summary>
        /// Mark the running status of a specific task. This method is guaranteed to run on the main scheduler thread.
        /// </summary>
        /// <param name="identifier"></param>
        void MarkAsRunning(T identifier);
        /// <summary>
        /// Mark the completed status of a specific task. This method is guaranteed to run on the main scheduler thread.
        /// </summary>
        /// <param name="identifier"></param>
        void MarkAsCompleted(T identifier);
    }
}
