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

using RenderWareLib;


namespace Demo.Apps
{
    public partial class RWStructureViewer : Form
    {
        private List<RWSection> sections = new List<RWSection>();

        public RWStructureViewer()
        {
            InitializeComponent();
            this.openToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
        }

        private void OpenFile(string filePath)
        {
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    RWSectionHeader outHeader;
                    while (RWSectionHeader.ReadSectionHeader(br, out outHeader))
                    {
                        RWSection sec = RWSection.ReadSectionBody(br, outHeader);
                        sections.Add(sec);
                        if (br.BaseStream.Position + 12 > br.BaseStream.Length)
                        {
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < sections.Count; i += 1)
            {
                AddNode(sections[i], null);
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "Gta RenderWare Binary Stream File (*.dff;*.txd)|*.dff;*.txd";
                dlg.Multiselect = false;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    OpenFile(dlg.FileName);
                }
            }
        }

        void AddNode(RWSection sec, TreeNode node = null)
        {
            TreeNode child = new TreeNode(sec.GetDescription());
            child.Tag = sec;
            if (node != null)
            {
                node.Nodes.Add(child);
            }
            else
            {
                treeView1.Nodes.Add(child);
            }
            if (sec.Children.Count > 0)
            {
                for (int i = 0; i < sec.Children.Count; i += 1)
                {
                    AddNode(sec.Children[i], child);
                }
            }
        }
    }
}
