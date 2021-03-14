using Eto.Forms;
using Grasshopper;
using Grasshopper.Kernel;
using PPGHAdapter.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGHAdapter.GH
{
    public class PPGHAdapterPriority : GH_AssemblyPriority
    {
        private static FormMain _main = new FormMain();
        public override GH_LoadingInstruction PriorityLoad()
        {
            Instances.CanvasCreated += sender =>
            {
                _main.Show();
            };

            return GH_LoadingInstruction.Proceed;
        }
    }
}
