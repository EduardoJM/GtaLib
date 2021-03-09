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

using GtaLib.Squish;

using GtaLib.Experimental.ForceReader;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            treeView1.AfterSelect += TreeView1_AfterSelect;

            using (FileStream fs = File.Open(@"C:\Users\Eduardo\Documents\Mods\San_Andreas_Farming_Equipment_DLC\San Andreas Farming Equipment DLC\combine.dff", FileMode.Open))
            // using (FileStream fs = File.Open(@"C:\Users\Eduardo\Documents\Mods\1542580207_Honda_Prelude_Si_1994_Avant\HQLM\cadrona.dff", FileMode.Open))
            { 
                using (BinaryReader br = new BinaryReader(fs))
                {
                    ForceDFFReader fdr = new ForceDFFReader(br);
                    RWSection sec = fdr.Read();
                    sec.SetVersion(0x1803FFFF, true);
                    AddNode(sec);

                    RWSection frameList = sec.FindChild(RWSectionId.RW_SECTION_FRAMELIST);
                    // RWSection[] geometries = geoList.FindChildCollection(RWSectionId.RW_SECTION_GEOMETRY);
                    RWSection frameListStruct = frameList.FindChild(RWSectionId.RW_SECTION_STRUCT);

                    /*
                    List<byte> output = new List<byte>();
                    using (MemoryStream frameMs = new MemoryStream(frameListStruct.Data))
                    {
                        using (BinaryReader frameBr = new BinaryReader(frameMs))
                        {
                            uint frames = frameBr.ReadUInt32();
                            output.AddRange(BitConverter.GetBytes(frames));
                            for (int i = 0; i < frames; i += 1)
                            {
                                float[] row1 = new float[3];
                                row1[0] = frameBr.ReadSingle();
                                row1[1] = frameBr.ReadSingle();
                                row1[2] = frameBr.ReadSingle();
                                float row1Length = (float)Math.Sqrt(row1[0] * row1[0] + row1[1] * row1[1] + row1[2] * row1[2]);
                                row1[0] /= row1Length;
                                row1[1] /= row1Length;
                                row1[2] /= row1Length;
                                System.Diagnostics.Debug.Print("Row 1 Length: " + row1Length);
                                float[] row2 = new float[3];
                                row2[0] = frameBr.ReadSingle();
                                row2[1] = frameBr.ReadSingle();
                                row2[2] = frameBr.ReadSingle();
                                float row2Length = (float)Math.Sqrt(row2[0] * row2[0] + row2[1] * row2[1] + row2[2] * row2[2]);
                                row2[0] /= row2Length;
                                row2[1] /= row2Length;
                                row2[2] /= row2Length;
                                System.Diagnostics.Debug.Print("Row 2 Length: " + row2Length);
                                float[] row3 = new float[3];
                                row3[0] = frameBr.ReadSingle();
                                row3[1] = frameBr.ReadSingle();
                                row3[2] = frameBr.ReadSingle();
                                float row3Length = (float)Math.Sqrt(row3[0] * row3[0] + row3[1] * row3[1] + row3[2] * row3[2]);
                                row3[0] /= row3Length;
                                row3[1] /= row3Length;
                                row3[2] /= row3Length;
                                System.Diagnostics.Debug.Print("Row 3 Length: " + row3Length);

                                output.AddRange(BitConverter.GetBytes(row1[0]));
                                output.AddRange(BitConverter.GetBytes(row1[1]));
                                output.AddRange(BitConverter.GetBytes(row1[2]));
                                output.AddRange(BitConverter.GetBytes(row2[0]));
                                output.AddRange(BitConverter.GetBytes(row2[1]));
                                output.AddRange(BitConverter.GetBytes(row2[2]));
                                output.AddRange(BitConverter.GetBytes(row3[0]));
                                output.AddRange(BitConverter.GetBytes(row3[1]));
                                output.AddRange(BitConverter.GetBytes(row3[2]));

                                output.AddRange(frameBr.ReadBytes(12 + 8));
                                // frameBr.BaseStream.Position += 12 + 8;
                            }
                        }
                    }
                    frameListStruct.Data = output.ToArray();
                    */

                    sec.RecalculateSize();


                    using (FileStream wfs = File.Create(@"C:\Users\Eduardo\Documents\Mods\San_Andreas_Farming_Equipment_DLC\San Andreas Farming Equipment DLC\combine_unlock.dff"))
                    // using (FileStream wfs = File.Create(@"C:\Users\Eduardo\Documents\Mods\1542580207_Honda_Prelude_Si_1994_Avant\HQLM\cadrona_demo.dff"))
                    {
                        using (BinaryWriter bw = new BinaryWriter(wfs))
                        {
                            sec.Write(bw);
                        }
                    }
                    

                    /*
                    ForceTXDReader ftr = new ForceTXDReader(br);
                    RWSection sec = ftr.Read();
                    AddNode(sec);

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
                                this.pictureBox1.Image = arc.Textures[2].GetBitmapImage(0);
                            }
                        }
                    }
                    */

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

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }
            RWSection sec = e.Node.Tag as RWSection;
            if (sec == null)
            {
                return;
            }
            if (sec.Header.Id == RWSectionId.RW_SECTION_STRING ||
                sec.Header.Id == RWSectionId.RW_SECTION_FRAME)
            {
                textBox1.Text = Encoding.ASCII.GetString(sec.Data);
            }
        }

        void AddNode(RWSection sec, TreeNode node = null)
        {
            TreeNode child = new TreeNode(sec.GetDescription());
            child.Tag = sec;
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
