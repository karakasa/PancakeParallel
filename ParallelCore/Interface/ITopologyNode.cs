using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelCore.Interface
{
    public interface ITopologyNode
    {
        ICollection<ITopologyNode> Sources { get; }
        void Compute();
    }
}
