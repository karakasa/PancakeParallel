using Grasshopper.Kernel;
using ParallelCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGHAdapter.Wrapper
{
    public class Component : EditableTopologicalNode
    {
        public GH_Component MappedComponent { get; }

        public Component(GH_Component comp)
        {
            MappedComponent = comp;
        }

        public override void Compute()
        {
            MappedComponent.ClearData();
            MappedComponent.CollectData();
            MappedComponent.ComputeData();
        }
    }
}
