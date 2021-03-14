using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGHAdapter.GH
{
    public class PPGHAdapterInfo : GH_AssemblyInfo
    {
        public override string Name => "Pancake GH Parallel Solver";

        public override Bitmap Icon => null;
        public override string Description => "Compute Grasshopper definition in a multi-threaded manner, which may boost calculation.";

        public static readonly Guid PPGHAGuid = new Guid("c6c19589-ab63-4b60-8d7f-2c1b6d60ffff");
        public override Guid Id => PPGHAGuid;

        public override string AuthorName => "Keyu Gan";
        public override string AuthorContact => "pancake@gankeyu.com";

        public override string Version => "1.0.0";
        public override GH_LibraryLicense License => GH_LibraryLicense.free;
    }
}
