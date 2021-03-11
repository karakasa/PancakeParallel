using NUnit.Framework;
using ParallelCore.Scheduler;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace UnitTest
{
    [TestFixture]
    public class SequentialTasks
    {
        [Test]
        public void Sync()
        {
            var sum = 0;
            var scheduler = SchedulerFactory.CreateSequentialTaskScheduler(101, i => sum += i);

            scheduler.RunSynchronously();
            Assert.AreEqual(sum, 5050);
        }
        [Test]
        public void Async()
        {
            var result = new int[101];
            var scheduler = SchedulerFactory.CreateSequentialTaskScheduler(101, i => result[i] = i);

            scheduler.Run();
            Assert.AreEqual(result.Sum(), 5050);
        }
        [Test]
        public void ExceptionTest()
        {
            var scheduler = SchedulerFactory.CreateSequentialTaskScheduler(101, i => throw new NotSupportedException());

            scheduler.Run();
            var exceptions = scheduler.Exceptions.ToArray();
            Assert.AreEqual(exceptions.Select(ex => ex.Identifier).Sum(), 5050);

            foreach (var it in exceptions)
                Assert.IsAssignableFrom<NotSupportedException>(it.InnerException);

            scheduler = SchedulerFactory.CreateSequentialTaskScheduler(101, i => throw new NotSupportedException());

            scheduler.RunSynchronously();
            exceptions = scheduler.Exceptions.ToArray();
            Assert.AreEqual(exceptions.Select(ex => ex.Identifier).Sum(), 5050);

            foreach (var it in exceptions)
                Assert.IsAssignableFrom<NotSupportedException>(it.InnerException);
        }
    }
}
