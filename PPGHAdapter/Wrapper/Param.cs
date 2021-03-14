using Grasshopper.Kernel;
using ParallelCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGHAdapter.Wrapper
{
    public class Param : EditableTopologicalNode
    {
        public IGH_Param MappedParam { get; }

        public Param(IGH_Param param)
        {   
            MappedParam = param;
        }

        public override void Compute()
        {
            MappedParam.ClearData();
            MappedParam.CollectData();
            MappedParam.ComputeData();
        }
    }
}
