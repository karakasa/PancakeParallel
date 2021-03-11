using ParallelCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelCore.Scheduler
{
    public static class SchedulerFactory
    {
        public static IScheduler<int> CreateSequentialTaskScheduler(int taskCount, Action<int> procedure)
        {
            var provider = new SequentialTaskProvider(taskCount, procedure);
            return Create(provider);
        }
        public static IScheduler<T> Create<T>(ITaskProvider<T> provider)
        {
            return new LoadBalancedScheduler<T>(provider);
        }
    }
}
