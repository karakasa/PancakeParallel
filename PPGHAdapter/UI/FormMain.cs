using Grasshopper;
using Grasshopper.Kernel;
using PPGHAdapter.GH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PPGHAdapter.UI
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ParallelSolver.SolveCurrentDocument();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ParallelSolver.RunSync();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ParallelSolver.RunAsync();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (var it in Instances.ActiveCanvas.Document.Objects.OfType<IGH_ActiveObject>())
                it.ClearData();

            Instances.ActiveCanvas.Invalidate();
        }
    }
}
