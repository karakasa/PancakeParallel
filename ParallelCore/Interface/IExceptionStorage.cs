using ParallelCore.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelCore.Interface
{
    public interface IExceptionStorage<T>
    {
        IEnumerable<TaskException<T>> Exceptions { get; }
    }
}
