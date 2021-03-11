using ParallelCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelCore.Scheduler
{
    public class TopologyTaskProvider : ITaskProvider<int>
    {
        public void MarkAsCompleted(int identifier)
        {
            throw new NotImplementedException();
        }

        public void MarkAsRunning(int identifier)
        {
            throw new NotImplementedException();
        }

        public void RunTaskSynchronously(int identifier)
        {
            throw new NotImplementedException();
        }

        public TaskProviderState TryGetNextAvailableTask(out int identifier)
        {
            throw new NotImplementedException();
        }
    }
}
