using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelCore.Scheduler
{
    public enum TaskProviderState
    {
        Normal,
        NoAvailableTask,
        AllComplete,
        Abort
    }
    public enum TaskStatus
    {
        Pending,
        Running,
        Completed
    }
    public struct TaskException<T>
    {
        public T Identifier;
        public Exception InnerException;
    }
}
