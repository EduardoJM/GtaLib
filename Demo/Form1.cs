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

using GtaLib.Experimental.ForceReader;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            using (FileStream fs = File.Open(@"C:\Users\Eduardo\Documents\Mods\San_Andreas_Farming_Equipment_DLC\San Andreas Farming Equipment DLC\combine.txd", FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    ForceTXDReader ftr = new ForceTXDReader(br);
                    RWSection sec = ftr.Read();
                    AddNode(sec);

                    /*
                    using (FileStream wfs = File.Create(@"C:\Users\Eduardo\Documents\Mods\San_Andreas_Farming_Equipment_DLC\San Andreas Farming Equipment DLC\combine_unlock.txd"))
                    {
                        using(BinaryWriter bw = new BinaryWriter(wfs))
                        {
                            sec.Write(bw);
                        }
                    }
                    */

                    /*
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (BinaryWriter bw = new BinaryWriter(ms))
                        {
                            sec.Write(bw);
                        }
                        using (MemoryStream ms2 = new MemoryStream(ms.ToArray()))
                        {
                            using (BinaryReader mBr = new BinaryReader(ms2))
                            {
                                TXDArchive arc = new TXDArchive();
                                arc.Read(mBr);
                                MessageBox.Show(arc.Textures.Count.ToString());
                            }
                        }
                    }
                    */

                    /*
                    RWSectionHeader header;
                    if (TXDArchive.GotoTextureDictionarySection(br, out header))
                    {
                        RWSection sec = RWSection.ReadSectionBody(br, header);
                        // RWSection structSec = sec.FindChild(RWSectionId.RW_SECTION_STRUCT);

                        AddNode(sec);
                    }
                    */
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
                for (int i = 0; i < sec.Children.Count; i += 1)
                {
                    AddNode(sec.Children[i], child);
                }
            }
        }

    }
}
