using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using RenderWareLib;

namespace GtaLib.Experimental.ForceReader
{
    public class ForceDFFReader
    {
        private BinaryReader br;

        private RWSectionHeader Header { get; set; }
        private RWSectionHeader PreviousHeader { get; set; }

        private RWSection Clump { get; set; }

        private RWSection FrameList { get; set; }

        private RWSection GeometryList { get; set; }

        private RWSection CurrentGeometry { get; set; }

        private RWSection CurrentGeometryExtension { get; set; }

        private RWSection CurrentMaterialList { get; set; }

        private RWSection CurrentMaterial { get; set; }

        private RWSection CurrentTexture { get; set; }

        private RWSection CurrentMaterialExtension { get; set; }

        private RWSection CurrentAtomic { get; set; }

        private RWSection CurrentAtomicExtension { get; set; }

        public ForceDFFReader(BinaryReader br)
        {
            this.br = br;
        }

        public RWSection Read()
        {
            ReadHeader();
            Clump.RecalculateSize();
            return Clump;
        }

        private void ReadHeader()
        {
            PreviousHeader = Header;
            if (br.BaseStream.Position + 12 < br.BaseStream.Length)
            {
                Header = new RWSectionHeader((RWSectionId)br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32());
                // System.Diagnostics.Debug.Print("Reading Header: " + Header.Id.ToString());
                ParseSection();
            }
            else
            {
                // System.Diagnostics.Debug.Print("Ending Size: " + br.BaseStream.Position + " - " + br.BaseStream.Length);
            }
        }

        private void ParseSection()
        {
            switch (Header.Id)
            {
                case RWSectionId.RW_SECTION_STRUCT:
                    ParseData();
                    break;
                case RWSectionId.RW_SECTION_STRING:
                    ParseUnused();
                    break;
                case RWSectionId.RW_SECTION_EXTENSION:
                    ParseExtension();
                    break;
                case RWSectionId.RW_SECTION_TEXTURE:
                    ParseTexture();
                    break;
                case RWSectionId.RW_SECTION_MATERIAL:
                    ParseMaterial();
                    break;
                case RWSectionId.RW_SECTION_MATERIALLIST:
                    ParseMaterialList();
                    break;
                case RWSectionId.RW_SECTION_FRAMELIST:
                    ParseFrameList();
                    break;
                case RWSectionId.RW_SECTION_GEOMETRY:
                    ParseGeometry();
                    break;
                case RWSectionId.RW_SECTION_CLUMP:
                    ParseClump();
                    break;
                case RWSectionId.RW_SECTION_ATOMIC:
                    ParseAtomic();
                    break;
                case RWSectionId.RW_SECTION_GEOMETRYLIST:
                    ParseGeometryList();
                    break;
                case RWSectionId.RW_SECTION_MATERIALSPLIT:
                    ParseMaterialSplit();
                    break;
                case RWSectionId.RW_SECTION_MATERIAL_EFFECTS_PLG:
                    ParseMaterialEffectsPLG();
                    break;
                case RWSectionId.RW_SECTION_REFLECTION_MATERIAL:
                    ParseReflection();
                    break;
                case RWSectionId.RW_SECTION_SPECULAR_MATERIAL:
                    ParseSpecular();
                    break;
                case RWSectionId.RW_SECTION_MESH_EXTENSION:
                    ParseBreakable();
                    break;
                case RWSectionId.RW_SECTION_NIGHT_VERTEX_COLORS:
                    ParseNightVertexColor();
                    break;
                case RWSectionId.RW_SECTION_RIGHT_TO_RENDER:
                    ParseRightToRender();
                    break;
                case RWSectionId.RW_SECTION_COLLISION_MODEL:
                    ParseCollisionModel();
                    break;
                case RWSectionId.RW_SECTION_INVALID:
                case RWSectionId.RW_SECTION_FRAME:
                    ParseUnused();
                    break;
                default:
                    ParseUnused();
                    break;
            }
        }

        void ParseData()
        {
            switch (PreviousHeader.Id)
            {
                case RWSectionId.RW_SECTION_TEXTURE:
                    ParseTextureData();
                    break;
                case RWSectionId.RW_SECTION_MATERIALLIST:
                    ParseMaterialListData();
                    break;
                case RWSectionId.RW_SECTION_FRAMELIST:
                    ParseFrameListData();
                    break;
                case RWSectionId.RW_SECTION_GEOMETRY:
                    ParseGeometryData();
                    break;
                case RWSectionId.RW_SECTION_CLUMP:
                    ParseClumpData();
                    break;
                case RWSectionId.RW_SECTION_ATOMIC:
                    ParseAtomicData();
                    break;
                case RWSectionId.RW_SECTION_GEOMETRYLIST:
                    ParseGeometryListData();
                    break;
                default:
                    ParseUnused();
                    break;
            }
        }

        private void ParseTexture()
        {
            CurrentTexture = new RWSection(RWSectionId.RW_SECTION_TEXTURE, CurrentMaterial);
            ReadHeader();
        }

        private void ParseUnused()
        {
            System.Diagnostics.Debug.Print("Unused: " + Header.Id.ToString());
            if (br.BaseStream.Position + Header.Size > br.BaseStream.Length)
            {
            }
            else
            {
                br.BaseStream.Position += Header.Size;
            }
            ReadHeader();
        }

        private void ParseMaterialSplit()
        {
            // System.Diagnostics.Debug.Print("MaterialSplit");
            List<byte> data = new List<byte>();
            data.AddRange(br.ReadBytes(4));
            int numMeshs = br.ReadInt32();
            data.AddRange(BitConverter.GetBytes(numMeshs));
            data.AddRange(br.ReadBytes(4));
            for (int i = 0; i < numMeshs; i++)
            {
                int numIndices = br.ReadInt32();
                data.AddRange(BitConverter.GetBytes(numIndices));
                data.AddRange(br.ReadBytes(4 + numIndices * 4));
            }
            if (CurrentGeometryExtension == null)
            {
                if (CurrentGeometry == null)
                {
                    ReadHeader();
                    return;
                }
                CurrentGeometryExtension = new RWSection(RWSectionId.RW_SECTION_EXTENSION, CurrentGeometry);
            }
            RWSection sec = new RWSection(RWSectionId.RW_SECTION_MATERIALSPLIT, CurrentGeometryExtension);
            sec.Data = data.ToArray();
            ReadHeader();
        }
        private void ParseMaterialList()
        {
            // System.Diagnostics.Debug.Print("MaterialList");
            CurrentMaterialList = new RWSection(RWSectionId.RW_SECTION_MATERIALLIST, CurrentGeometry);
            ReadHeader();
        }

        private void ParseMaterial()
        {
            //br.BaseStream.Position -= 12L;
            CurrentMaterialExtension = null;
            CurrentMaterial = new RWSection(RWSectionId.RW_SECTION_MATERIAL, CurrentMaterialList);
            RWSection sec = new RWSection(br.ReadBytes(40));
            CurrentMaterial.AddChild(sec);
            /*
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            br.ReadInt32();
            */
            ReadHeader();
        }
        private void ParseGeometryList()
        {
            // System.Diagnostics.Debug.Print("GeometryList");
            GeometryList = new RWSection(RWSectionId.RW_SECTION_GEOMETRYLIST, Clump);
            ReadHeader();
        }

        private void ParseGeometry()
        {
            // System.Diagnostics.Debug.Print("Geometry");
            CurrentGeometryExtension = null;
            CurrentGeometry = new RWSection(RWSectionId.RW_SECTION_GEOMETRY, GeometryList);
            ReadHeader();
        }

        private void ParseFrameList()
        {
            // System.Diagnostics.Debug.Print("FrameList");
            FrameList = new RWSection(RWSectionId.RW_SECTION_FRAMELIST, Clump);
            ReadHeader();
        }

        private void ParseExtension()
        {
            // System.Diagnostics.Debug.Print("Extension");
            // System.Diagnostics.Debug.Print(PreviousHeader.Id.ToString());
            // System.Diagnostics.Debug.Print(Header.Size.ToString());

            /*
            if (CurrentMaterial != null && CurrentTexture == null)
            {
                CurrentMaterialExtension = new RWSection(RWSectionId.RW_SECTION_EXTENSION, CurrentMaterial);
            }
            else
            {
                System.Diagnostics.Debug.Print("Not Material Extension!!!");
            }
            */
            ReadHeader();
        }

        private void ParseClump()
        {
            // System.Diagnostics.Debug.Print("Clump");
            // uint version = 0x1803FFFF;
            uint version = Header.Version;
            Clump = new RWSection(RWSectionId.RW_SECTION_CLUMP, version);
            ReadHeader();
        }

        private void ParseAtomic()
        {
            // System.Diagnostics.Debug.Print("Atomic");
            CurrentAtomic = new RWSection(RWSectionId.RW_SECTION_ATOMIC, Clump);
            CurrentMaterial = null;
            CurrentMaterialExtension = null;
            ReadHeader();
        }

        private void ParseTextureData()
        {
            // System.Diagnostics.Debug.Print("TextureData");
            br.BaseStream.Position -= 12L;
            RWSection sec = new RWSection(br.ReadBytes(16));
            CurrentTexture.AddChild(sec);
            // br.ReadInt16();
            // br.ReadInt16();
            if (br.ReadInt32() == 2)
            {
                uint size = br.ReadUInt32();
                br.BaseStream.Position -= 8L;
                RWSection str1Sec = new RWSection(br.ReadBytes((int)size + 12));
                CurrentTexture.AddChild(str1Sec);
                // Header = new RWSectionHeader(RWSectionId.RW_SECTION_STRING, br.ReadUInt32(), br.ReadUInt32());
                // System.Diagnostics.Debug.Print("String: " + Encoding.ASCII.GetString(br.ReadBytes((int)Header.Size)));
                // clump.GeometryList.Geometry[geometryPos].MaterialList.Materials[clump.GeometryList.Geometry[geometryPos].MaterialList.MaterialPos - 1].Texture.Strings[0] = new RWString(commonTools.getNullTerminatedString(commonTools.getStringFromBytes(br.ReadBytes(Header.SectionSize))));
                if (br.ReadInt32() == 2)
                {
                    // Header = new RWSectionHeader(RWSectionId.RW_SECTION_STRING, br.ReadUInt32(), br.ReadUInt32());
                    size = br.ReadUInt32();
                    br.BaseStream.Position -= 8L;
                    RWSection str2Sec = new RWSection(br.ReadBytes((int)size + 12));
                    CurrentTexture.AddChild(str2Sec);

                    // System.Diagnostics.Debug.Print("String: " + Encoding.ASCII.GetString());
                    // clump.GeometryList.Geometry[geometryPos].MaterialList.Materials[clump.GeometryList.Geometry[geometryPos].MaterialList.MaterialPos - 1].Texture.Strings[1] = new RWString(commonTools.getNullTerminatedString(commonTools.getStringFromBytes(br.ReadBytes(Header.SectionSize))));
                    if (br.ReadInt32() == 3)
                    {
                        // Header = new RWSectionHeader(RWSectionId.RW_SECTION_EXTENSION, br.ReadUInt32(), br.ReadUInt32());
                        // br.BaseStream.Position += Header.Size;
                        size = br.ReadUInt32();
                        br.BaseStream.Position -= 8L;
                        RWSection extSec = new RWSection(br.ReadBytes((int)size + 12));
                        CurrentTexture.AddChild(extSec);
                        // clump.GeometryList.Geometry[geometryPos].MaterialList.Materials[clump.GeometryList.Geometry[geometryPos].MaterialList.MaterialPos - 1].Texture.Extension = new RWExtension(br.ReadBytes(Header.SectionSize));
                    }
                    else
                    {
                        br.BaseStream.Position -= 4L;
                    }
                }
                else
                {
                    br.BaseStream.Position -= 4L;
                }
            }
            else
            {
                br.BaseStream.Position -= 4L;
            }
            CurrentTexture = null;
            ReadHeader();
        }

        private void ParseMaterialListData()
        {
            // System.Diagnostics.Debug.Print("MaterialListData");
            int materialCount = br.ReadInt32();
            br.BaseStream.Position -= 16L;
            RWSection sec = new RWSection(br.ReadBytes(16 + materialCount * 4));
            CurrentMaterialList.AddChild(sec);
            /*
            for (int i = 0; i < materialCount; i++)
            {
                br.ReadInt32();
            }
            */
            ReadHeader();
        }

        private void ParseFrameListData()
        {
            // System.Diagnostics.Debug.Print("FrameListData");

            int frameCount = br.ReadInt32();
            int num = 0;
            List<byte> data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(frameCount));
            for (num = 0; num < frameCount; num++)
            {
                // clump.FrameList.FramesData[num] = new RWFrameListData();
                // br.BaseStream.Position += 36 + 12 + 8;
                data.AddRange(br.ReadBytes(56));
            }
            RWSection secStruct = new RWSection(RWSectionId.RW_SECTION_STRUCT, FrameList);
            secStruct.Data = data.ToArray();
            // List<RWSection> otherChilds = new List<RWSection>();
            for (num = 0; num < frameCount; num++)
            {
                if (br.ReadInt32() == 3)
                {
                    uint size = br.ReadUInt32();
                    br.BaseStream.Position -= 8L;
                    // Header = new RWSectionHeader(RWSectionId.RW_SECTION_EXTENSION, br.ReadUInt32(), br.ReadUInt32());
                    RWSection frameSec = new RWSection(br.ReadBytes((int)size + 12));
                    FrameList.AddChild(frameSec);
                    /*
                    data.AddRange(BitConverter.GetBytes((uint)Header.Id));
                    data.AddRange(BitConverter.GetBytes((uint)Header.Size));
                    data.AddRange(BitConverter.GetBytes((uint)Header.Version));
                    data.AddRange(br.ReadBytes((int)Header.Size));
                    */
                    // br.BaseStream.Position += Header.Size;
                    //clump.FrameList.Extensions[num] = new RWExtension(br.ReadBytes(Header.SectionSize));
                    continue;
                }
                br.BaseStream.Position -= 4L;
                break;
            }

            ReadHeader();
        }

        private void ParseClumpData()
        {
            // System.Diagnostics.Debug.Print("ClumpData");
            // int num = br.ReadInt32();
            // System.Diagnostics.Debug.Print(num + "Atomics");
            //System.Diagnostics.Debug.Print("Version Header " + header.Version);
            //System.Diagnostics.Debug.Print("Version Size " + header.Size);
            //if (header.Version == 268697599 || header.Version == 402915327 || header.Version == 469893130)
            {
                // TODO: fix this.
                // br.BaseStream.Position += 8L;
            }

            RWSection sec = new RWSection(RWSectionId.RW_SECTION_STRUCT, Clump);
            sec.Data = br.ReadBytes(12);

            ReadHeader();
        }

        private void ParseAtomicData()
        {
            // System.Diagnostics.Debug.Print("AtomicData");
            br.BaseStream.Position -= 12L;
            RWSection sec = new RWSection(br.ReadBytes(12 + 4 * 4));
            CurrentAtomic.AddChild(sec);

            CurrentAtomicExtension = new RWSection(RWSectionId.RW_SECTION_EXTENSION, CurrentAtomic);

            ReadHeader();
        }

        private void ParseGeometryListData()
        {
            // System.Diagnostics.Debug.Print("GeometryListData");
            RWSection sec = new RWSection(RWSectionId.RW_SECTION_STRUCT, GeometryList);
            sec.Data = br.ReadBytes(4);
            // br.BaseStream.Position += 4;

            ReadHeader();
        }

        private void ParseGeometryData()
        {
            // System.Diagnostics.Debug.Print("GeometryData");
            br.BaseStream.Position -= 12L;
            // br.BaseStream.Position += Header.Size;
            RWSection sect = new RWSection(br.ReadBytes((int)Header.Size + 12));
            CurrentGeometry.AddChild(sect);

            ReadHeader();
        }

        private void ParseMaterialEffectsPLG()
        {
            if (CurrentMaterialExtension == null)
            {
                if (CurrentMaterial == null)
                {
                    br.BaseStream.Position += (int)Header.Size;
                    ReadHeader();
                    System.Diagnostics.Debug.Print("JUMP: MATERIAL_EFFECTS_PLG");
                    return;
                }
                CurrentMaterialExtension = new RWSection(RWSectionId.RW_SECTION_EXTENSION, CurrentMaterial);
            }
            br.BaseStream.Position -= 12L;
            RWSection sect = new RWSection(br.ReadBytes((int)Header.Size + 12));
            CurrentMaterialExtension.AddChild(sect);

            ReadHeader();
        }

        private void ParseReflection()
        {
            if (CurrentMaterialExtension == null)
            {
                if (CurrentMaterial == null)
                {
                    br.BaseStream.Position += 24;
                    ReadHeader();
                    return;
                }
                CurrentMaterialExtension = new RWSection(RWSectionId.RW_SECTION_EXTENSION, CurrentMaterial);
            }
            br.BaseStream.Position -= 12L;
            RWSection sect = new RWSection(br.ReadBytes(36));
            CurrentMaterialExtension.AddChild(sect);

            ReadHeader();
        }
        private void ParseSpecular()
        {
            if (CurrentMaterialExtension == null)
            {
                if (CurrentMaterial == null)
                {
                    br.BaseStream.Position += 28;
                    ReadHeader();
                    return;
                }
                CurrentMaterialExtension = new RWSection(RWSectionId.RW_SECTION_EXTENSION, CurrentMaterial);
            }
            br.BaseStream.Position -= 12L;
            RWSection sect = new RWSection(br.ReadBytes(40));
            CurrentMaterialExtension.AddChild(sect);

            ReadHeader();
        }

        private void ParseBreakable()
        {
            if (CurrentGeometryExtension == null)
            {
                if (CurrentGeometry == null)
                {
                    br.BaseStream.Position += (int)Header.Size;
                    ReadHeader();
                    return;
                }
                CurrentGeometryExtension = new RWSection(RWSectionId.RW_SECTION_EXTENSION, CurrentGeometry);
            }
            br.BaseStream.Position -= 12L;
            RWSection sec = new RWSection(br.ReadBytes((int)Header.Size + 12));
            CurrentGeometryExtension.AddChild(sec);

            ReadHeader();
        }

        private void ParseNightVertexColor()
        {
            if (CurrentGeometryExtension == null)
            {
                if (CurrentGeometry == null)
                {
                    br.BaseStream.Position += (int)Header.Size;
                    ReadHeader();
                    return;
                }
                CurrentGeometryExtension = new RWSection(RWSectionId.RW_SECTION_EXTENSION, CurrentGeometry);
            }
            br.BaseStream.Position -= 12L;
            RWSection sec = new RWSection(br.ReadBytes((int)Header.Size + 12));
            CurrentGeometryExtension.AddChild(sec);

            ReadHeader();
        }

        private void ParseRightToRender()
        {
            br.BaseStream.Position -= 12L;
            RWSection sec = new RWSection(br.ReadBytes(20));

            CurrentAtomicExtension.AddChild(sec);

            ReadHeader();
        }

        private void ParseCollisionModel()
        {
            CurrentAtomicExtension = null;
            CurrentAtomic = null;

            br.BaseStream.Position -= 12L;
            RWSection sec = new RWSection(RWSectionId.RW_SECTION_EXTENSION, Clump);

            RWSection col = new RWSection(br.ReadBytes(12 + (int)Header.Size));
            sec.AddChild(col);

            ReadHeader();
        }

    }
}
