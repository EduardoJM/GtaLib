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
using RenderWareLib.SectionsData;
using RenderWareLib.SectionsData.TXD;
using RenderWareLib.SectionsData.DFF;
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
                    sec.SetVersion(RWVersion.RW_VERSION_GTASA, true);
                    AddNode(sec);

                    RWSection geomList = sec.FindChild(RWSectionId.RW_SECTION_GEOMETRYLIST);

                    if (geomList != null)
                    {
                        RWSection[] geoCollection = geomList.FindChildCollection(RWSectionId.RW_SECTION_GEOMETRY);
                        for (int i = 0; i < geoCollection.Length; i += 1)
                        {
                            RWSection geo = geoCollection[i];
                            RWSection geoStruct = geo.FindChild(RWSectionId.RW_SECTION_STRUCT);
                            RWGeometryData dt = geoStruct.GetParsedData() as RWGeometryData;
                            // MessageBox.Show(dt.VertexCount.ToString());

                            RWSection extension = geo.FindChild(RWSectionId.RW_SECTION_EXTENSION);
                            RWSection matSplit = extension.FindChild(RWSectionId.RW_SECTION_MATERIALSPLIT);

                            RWMaterialSplitData splitData = matSplit.GetParsedData() as RWMaterialSplitData;
                            StringBuilder vertices = new StringBuilder();
                            StringBuilder uvs = new StringBuilder();
                            StringBuilder normals = new StringBuilder();
                            // vertices.AppendLine("g");
                            for (int k = 0; k < dt.Vertices.Length; k++)
                            {
                                vertices.AppendLine(string.Format("v {0} {1} {2}", dt.Vertices[k].X, dt.Vertices[k].Y, dt.Vertices[k].Z));
                                /*
                                if (dt.TexCoords.Length > 0)
                                {
                                    uvs.AppendLine(string.Format("vt {0} {1}", dt.TexCoords[0][k].X, dt.TexCoords[0][k].Y));
                                }
                                */
                                /*
                                if (dt.Normals.Length > 0)
                                {
                                    normals.AppendLine(string.Format("vn {0} {1} {2}", dt.Normals[k].X, dt.Normals[k].Y, dt.Normals[k].Z));
                                }
                                */
                            }
                            StringBuilder faces = new StringBuilder();
                            if (splitData.TriangleMode == DFFTriangleMode.TriangleList)
                            {
                                List<RWGeometryDataTriangle> triangles = new List<RWGeometryDataTriangle>();
                                for (int k = 0; k < splitData.Meshes.Length; k++)
                                {
                                    // faces.AppendLine("g mesh" + (k + 1).ToString());
                                    for (int n = 0; n < splitData.Meshes[k].Indices.Length; n += 3)
                                    {
                                        RWGeometryDataTriangle tri = new RWGeometryDataTriangle();
                                        tri.Vertex1 = (ushort)splitData.Meshes[k].Indices[n];
                                        tri.Vertex2 = (ushort)splitData.Meshes[k].Indices[n + 1];
                                        tri.Vertex3 = (ushort)splitData.Meshes[k].Indices[n + 2];
                                        tri.MaterialId = (ushort)splitData.Meshes[k].MaterialIndex;
                                        faces.AppendLine(string.Format("f {0} {1} {2}", splitData.Meshes[k].Indices[n], splitData.Meshes[k].Indices[n + 1], splitData.Meshes[k].Indices[n + 2]));
                                        triangles.Add(tri);
                                    }
                                }
                                // MessageBox.Show(dt.TriangleCount + " - " + triangles.Count);
                                dt.TriangleCount = (uint)triangles.Count;
                                dt.Triangles = triangles.ToArray();
                                geoStruct.Data = dt.GetData(geoStruct);
                            }
                            else
                            {
                                throw new Exception("Unsupported yet.");
                            }

                            string fileData = vertices.ToString() + "\r\n\r\n" + uvs.ToString() + "\r\n\r\n" + normals.ToString() + "\r\n\r\n\r\n" + faces.ToString();
                            File.WriteAllText(@"C:\Users\Eduardo\Documents\Mods\testing.obj", fileData.Replace(',', '.'));
                        }
                    }

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
