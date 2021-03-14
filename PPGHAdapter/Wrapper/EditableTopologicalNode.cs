using ParallelCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGHAdapter.Wrapper
{
    public abstract class EditableTopologicalNode : ITopologicalNode
    {
        internal readonly List<ITopologicalNode> SourceList = new List<ITopologicalNode>();
        ICollection<ITopologicalNode> ITopologicalNode.Sources => SourceList;

        public abstract void Compute();
    }
}
