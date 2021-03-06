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
using GtaLib.TXD;
using RenderWareLib;
using RenderWareLib.SectionsData.TXD;
using GtaLib.Renderer.TXD;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            using (FileStream fs = File.Open(@"C:\Users\Eduardo\Documents\Mods\GTA_V_Rotating_Ferris_Wheel\GTA V Rotating Ferris Wheel\ferriswheel_seat.txd", FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    TXDArchive archive = new TXDArchive();
                    archive.Read(br);
                    for (int i = 0; i < archive.Textures.Count; i += 1)
                    {
                        AddNode(archive.Textures[i].DiffuseName + " - " + archive.Textures[i].Compression.ToString());
                    }
                }
            }
        }

        void AddNode(string sec, TreeNode node = null)
        {
            TreeNode child = new TreeNode(sec);
            if (node != null)
            {
                node.Nodes.Add(child);
            } else
            {
                treeView1.Nodes.Add(child);
            }
        }

    }
}
