using Grasshopper;
using ParallelCore.Scheduler;
using PPGHAdapter.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGHAdapter.GH
{
    public static class ParallelSolver
    {
        private static GraphCreator _graph;
        public static void SolveCurrentDocument()
        {
            var doc = Instances.ActiveCanvas?.Document;
            if (doc is null) return;

            _graph = new GraphCreator(doc);
        }
        public static void RunSync()
        {
            if (_graph is null) return;

            var provider = new TopologicalTaskProvider(_graph.Nodes);
            var scheduler = SchedulerFactory.Create(provider);

            Instances.ActiveCanvas.Document.DestroyAttributeCache();

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            scheduler.RunSynchronously();
            sw.Stop();
            System.Windows.Forms.MessageBox.Show(sw.ElapsedMilliseconds.ToString());

            Instances.ActiveCanvas.Invalidate();
        }
        public static void RunAsync()
        {
            if (_graph is null) return;

            var provider = new TopologicalTaskProvider(_graph.Nodes);
            var scheduler = SchedulerFactory.Create(provider);

            Instances.ActiveCanvas.Document.DestroyAttributeCache();

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            scheduler.Run();
            sw.Stop();
            System.Windows.Forms.MessageBox.Show(sw.ElapsedMilliseconds.ToString());

            Instances.ActiveCanvas.Invalidate();
        }
    }
}
