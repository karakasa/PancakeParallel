using ParallelCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelCore.Scheduler
{
    public class TopologicalTaskProvider : ITaskProvider<int>
    {
        public TopologicalTaskProvider(ICollection<ITopologicalNode> nodes)
        {
            Initialize(nodes);
        }
        public void MarkAsCompleted(int identifier)
        {
            _nodeStatus[identifier] = TaskStatus.Completed;
        }

        public void MarkAsRunning(int identifier)
        {
            _nodeStatus[identifier] = TaskStatus.Running;
        }

        public void RunTaskSynchronously(int identifier)
        {
            _indexedNodes[identifier].Node.Compute();
        }

        public TaskProviderState TryGetNextAvailableTask(out int identifier)
        {
            var hasRuning = false;

            for (var i = 0; i < _indexedNodes.Length; i++)
            {
                switch (_nodeStatus[i])
                {
                    case TaskStatus.Pending:
                        identifier = i;
                        return TaskProviderState.Normal;
                    case TaskStatus.Running:
                        hasRuning = true;
                        break;
                    default:
                        break;
                }
            }

            identifier = -1;
            return hasRuning ? TaskProviderState.NoAvailableTask : TaskProviderState.AllComplete;
        }
        private struct AbstractNode
        {
            public int OriginalIndex;
            public ITopologicalNode Node;
            public List<ITopologicalNode> Sources;
        }

        private struct CachedNode
        {
            public ITopologicalNode Node;
            public int[] SourceIds;
        }
        private List<AbstractNode> DeepCopy(IEnumerable<AbstractNode> nodes)
        {
            return nodes.Select(n => new AbstractNode
            {
                OriginalIndex = n.OriginalIndex,
                Node = n.Node,
                Sources = new List<ITopologicalNode>(n.Sources)
            }).ToList();
        }
        private List<AbstractNode> TopologicalSort(List<AbstractNode> nodes)
        {
            var list = new List<AbstractNode>(nodes.Count);
            var copiedNodes = DeepCopy(nodes);

            for (; ; )
            {
                var found = false;

                for (var i = 0; i < copiedNodes.Count; i++)
                {
                    var it = copiedNodes[i];

                    if (it.Sources.Count == 0)
                    {
                        list.Add(nodes[it.OriginalIndex]);

                        for (var j = 0; j < copiedNodes.Count; j++)
                        {
                            if (i == j)
                                continue;

                            copiedNodes[j].Sources.RemoveAll(src => object.ReferenceEquals(src, it.Node));
                        }

                        copiedNodes.RemoveAt(i);
                        found = true;

                        break;
                    }
                }

                if (copiedNodes.Count == 0)
                    break;

                if (!found)
                {
                    // Loop in the graph
                    throw new InvalidOperationException("Loop detected in the topological graph.");
                }
            }

            return list;
        }

        private CachedNode[] ToIndexedNodes(List<AbstractNode> nodes)
        {
            var dict = new Dictionary<ITopologicalNode, int>();

            for (var i = 0; i < nodes.Count; i++)
                dict[nodes[i].Node] = i;

            return nodes.Select(node => new CachedNode
            {
                Node = node.Node,
                SourceIds = node.Sources.Select(src => dict[src]).ToArray()
            }).ToArray();
        }
        private CachedNode[] _indexedNodes;
        private TaskStatus[] _nodeStatus;
        private int _nextTask = 0;
        private void Initialize(ICollection<ITopologicalNode> nodes)
        {
            var _nodes = nodes.Select((n, i) => new AbstractNode
            {
                OriginalIndex = i,
                Node = n,
                Sources = n.Sources.ToList()
            }).ToList();

            _nodes = TopologicalSort(_nodes);
            _indexedNodes = ToIndexedNodes(_nodes);
            _nodeStatus = new TaskStatus[_indexedNodes.Length];
        }
    }
}
