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
        public TopologyTaskProvider(ICollection<ITopologyNode> nodes)
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
            public ITopologyNode Node;
            public List<ITopologyNode> Sources;
        }

        private struct CachedNode
        {
            public ITopologyNode Node;
            public int[] SourceIds;
        }
        private List<AbstractNode> TopologicalSort(List<AbstractNode> nodes)
        {
            var list = new List<AbstractNode>(nodes.Count);

            for (; ; )
            {
                var found = false;

                for (var i = 0; i < nodes.Count; i++)
                {
                    var it = nodes[i];

                    if (it.Sources.Count == 0)
                    {
                        list.Add(it);

                        for (var j = 0; j < nodes.Count; j++)
                        {
                            if (i == j)
                                continue;

                            nodes[j].Sources.RemoveAll(src => object.ReferenceEquals(src, it.Node));
                        }

                        nodes.RemoveAt(i);
                        found = true;

                        break;
                    }
                }

                if (nodes.Count == 0)
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
            var dict = new Dictionary<ITopologyNode, int>();

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
        private void Initialize(ICollection<ITopologyNode> nodes)
        {
            var _nodes = nodes.Select(n => new AbstractNode
            {
                Node = n,
                Sources = n.Sources.ToList()
            }).ToList();

            _nodes = TopologicalSort(_nodes);
            _indexedNodes = ToIndexedNodes(_nodes);
            _nodeStatus = new TaskStatus[_indexedNodes.Length];
        }
    }
}
