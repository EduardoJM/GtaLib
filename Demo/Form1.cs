using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GtaLib.DFF;
using RenderWareLib;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            using (FileStream fs = File.Open(@"C:\Users\Eduardo\Documents\Mods\GTA_V_Rotating_Ferris_Wheel\GTA V Rotating Ferris Wheel\ferriswheel_seat.dff", FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    RWSectionHeader header;
                    if (DFFLoader.GotoClumpSection(br, out header))
                    {
                        RWSection clump = RWSection.ReadSectionBody(br, header);
                        AddNode(clump);
                        // MessageBox.Show(clump.GetDescription());
                    }
                }
            }
        }

        void AddNode(RWSection sec, TreeNode node = null)
        {
            TreeNode child = new TreeNode(sec.GetDescription());
            if (node != null)
            {
                node.Nodes.Add(child);
            } else
            {
                treeView1.Nodes.Add(child);
            }
            if (sec.Children.Count > 0)
            {
                for (int i = 0; i < sec.Children.Count; i+= 1)
                {
                    AddNode(sec.Children[i], child);
                }
            }
        }

    }
}
