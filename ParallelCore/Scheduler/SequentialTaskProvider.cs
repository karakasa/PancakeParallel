using ParallelCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelCore.Scheduler
{
    public class SequentialTaskProvider : ITaskProvider<int>
    {
        private readonly int _totalTask;
        private readonly Action<int> _task;
        private int _nextTask = 0;
        public SequentialTaskProvider(int taskCount, Action<int> procedure)
        {
            _totalTask = taskCount;
            _task = procedure;
        }
        public void MarkAsCompleted(int identifier)
        {
        }

        public void MarkAsRunning(int identifier)
        {
        }

        public void RunTaskSynchronously(int identifier) => _task(identifier);

        public TaskProviderState TryGetNextAvailableTask(out int identifier)
        {
            if (_nextTask >= _totalTask)
            {
                identifier = default;
                return TaskProviderState.AllComplete;
            }

            identifier = _nextTask;
            _nextTask++;
            return TaskProviderState.Normal;
        }
    }
}
