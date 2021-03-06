using System.IO;

namespace RenderWareLib
{
    public class RWSectionHeader
    {
        public static bool IsContainerSection(RWSectionId id)
        {
            return (
                id == RWSectionId.RW_SECTION_CLUMP ||
                id == RWSectionId.RW_SECTION_FRAMELIST ||
                id == RWSectionId.RW_SECTION_EXTENSION ||
                id == RWSectionId.RW_SECTION_GEOMETRYLIST ||
                id == RWSectionId.RW_SECTION_GEOMETRY ||
                id == RWSectionId.RW_SECTION_MATERIALLIST ||
                id == RWSectionId.RW_SECTION_MATERIAL ||
                id == RWSectionId.RW_SECTION_TEXTURE || 
                id == RWSectionId.RW_SECTION_ATOMIC ||
                id == RWSectionId.RW_SECTION_TEXTUREDICTIONARY ||
                id == RWSectionId.RW_SECTION_TEXTURENATIVE
            );
        }

        public static bool ReadSectionHeader(BinaryReader br, out RWSectionHeader outHeader, RWSectionHeader lastRead = null)
        {
            outHeader = null;
            if (br.BaseStream.Position + 12L > br.BaseStream.Length)
            {
                System.Diagnostics.Debug.Print("Failed Stream Size");
                return false;
            }
            outHeader = new RWSectionHeader((RWSectionId)br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32());

            if (outHeader.Id == RWSectionId.RW_SECTION_STRUCT && lastRead != null)
            {
                if (lastRead.Id == RWSectionId.RW_SECTION_CLUMP)
                {
                    outHeader.Size = 12;
                }
                else if (lastRead.Id == RWSectionId.RW_SECTION_MATERIALLIST)
                {
                    outHeader.Size = 4;
                }
            }
            /*
            if (lastRead != null)
            {
                if (lastRead.Version != outHeader.Version
                    || ((uint)outHeader.Id == 0 && outHeader.Size == 0 && outHeader.Version == 0))
                {
                    System.Diagnostics.Debug.Print("Failed Checking");
                    return false;
                }
            }
            */
            return true;
        }

        public RWSectionId Id { get; set; }

        public uint Size { get; set; }

        public uint Version { get; set; }

        public RWSectionHeader()
        {
            Id = RWSectionId.RW_SECTION_INVALID;
            Size = 0;
            Version = 0;
        }

        public RWSectionHeader(RWSectionId _id, uint _size, uint _version)
        {
            Id = _id;
            Size = _size;
            Version = _version;
        }

        public RWSectionHeader(RWSectionHeader _other)
        {
            Id = _other.Id;
            Size = _other.Size;
            Version = _other.Version;
        }

        public bool IsContainer()
        {
            return IsContainerSection(Id);
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write((uint)Id);
            bw.Write((uint)Size);
            bw.Write((uint)Version);
        }
    }
}
