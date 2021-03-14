using NUnit.Framework;
using ParallelCore.Interface;
using ParallelCore.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace UnitTest
{
    [TestFixture]
    public class TopologicalTasks
    {
        private class DemoTask : ITopologicalNode
        {
            private Action<int> _procedure = null;
            public int Id;
            public DemoTask(int i)
            {
                Id = i;
            }

            public DemoTask(int i, Action<int> procedure) : this(i)
            {
                _procedure = procedure;
            }

            public List<ITopologicalNode> SourceNodes = new List<ITopologicalNode>();
            ICollection<ITopologicalNode> ITopologicalNode.Sources => SourceNodes;

            public void Compute()
            {
                _procedure(Id);
            }
        }

        private List<ITopologicalNode> SimpleNodes;

        [SetUp]
        public void SetUpSimpleData()
        {
            var task1 = new DemoTask(1);
            var task4 = new DemoTask(4);

            var task2 = new DemoTask(2);
            task2.SourceNodes.Add(task1);
            task2.SourceNodes.Add(task4);

            var task3 = new DemoTask(3);
            task3.SourceNodes.Add(task2);
            task3.SourceNodes.Add(task4);

            SimpleNodes = new List<ITopologicalNode> { task1, task2, task3, task4 };
        }
        [Test]
        public void CreateBasic()
        {
            Assert.DoesNotThrow(() => new TopologicalTaskProvider(SimpleNodes));
        }

        [Test]
        public void TopologicalSort()
        {
            TopologicalTaskProvider provider = null;

            Assert.DoesNotThrow(() => provider = new TopologicalTaskProvider(SimpleNodes));

            var obj = provider.GetType()
                .GetField("_indexedNodes", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(provider) as Array;

            Assert.AreEqual(obj.Length, 4);
            var objs = obj.Cast<object>()
                .Select(o => o.GetType().GetField("Node").GetValue(o) as DemoTask).ToArray();

            Assert.AreEqual(objs[0].Id, 1);
            Assert.AreEqual(objs[1].Id, 4);
            Assert.AreEqual(objs[2].Id, 2);
            Assert.AreEqual(objs[3].Id, 3);
        }

        [Test]
        public void CreateWithInvalidGraph()
        {
            // Loop

            var task1 = new DemoTask(0);
            var task2 = new DemoTask(0);

            task1.SourceNodes.Add(task2);
            task2.SourceNodes.Add(task1);

            var list = new List<ITopologicalNode> { task1, task2 };

            Assert.Throws<InvalidOperationException>(() => new TopologicalTaskProvider(list));
        }
        [Test]
        public void SimpleCalculation()
        {
            const int NUM = 100;
            const int TOTAL = (NUM + 1) * NUM / 2;

            var sum = new int[NUM];

            var list = new List<ITopologicalNode>(NUM);
            for (var i = 1; i <= NUM; i++)
            {
                var task = new DemoTask(i, n => sum[n - 1] = n);
                list.Add(task);
            }

            var provider = default(TopologicalTaskProvider);
            Assert.DoesNotThrow(() => provider = new TopologicalTaskProvider(list));

            var scheduler = SchedulerFactory.Create(provider);
            scheduler.Run();

            Assert.AreEqual(sum.Sum(), TOTAL);
        }
        private static Random rng = new Random();
        private static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        [Test]
        public void ComplexCalculation()
        {
            const int NUM = 100;
            const int TOTAL = (NUM + 1) * NUM / 2;
            const int LINKS = 500;

            var sum = new int[NUM];

            var list = new List<ITopologicalNode>(NUM);
            for (var i = 1; i <= NUM; i++)
            {
                var task = new DemoTask(i, n =>
                {
                    Thread.Sleep(100);
                    sum[n - 1] = n;
                });
                list.Add(task);
            }

            var rnd = new Random();
            for (var i = 0; i < LINKS; i++)
            {
                var to = rnd.Next(1, NUM);
                var from = rnd.Next(0, to - 1);

                if (list[to].Sources.Contains(list[from]))
                    continue;

                list[to].Sources.Add(list[from]);
            }

            Shuffle(list);

            var provider = default(TopologicalTaskProvider);
            Assert.DoesNotThrow(() => provider = new TopologicalTaskProvider(list));

            var scheduler = SchedulerFactory.Create(provider);
            scheduler.Run();

            Assert.AreEqual(sum.Sum(), TOTAL);
        }
    }
}
