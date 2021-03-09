using System.IO;

using RenderWareLib;

namespace GtaLib.DFF
{
    public class DFFLoader
    {
        /*
        struct IndexedDFFFrame
        {
            DFFFrame* frame;
            int32_t parentIdx;
            int32_t boneID;
        };
        */

        public static bool GotoClumpSection(BinaryReader br, out RWSectionHeader outHeader)
        {
            while (RWSectionHeader.ReadSectionHeader(br, out outHeader) && outHeader.Id != RWSectionId.RW_SECTION_CLUMP)
            {
                br.BaseStream.Position += outHeader.Size;
            }
            if (outHeader.Id == RWSectionId.RW_SECTION_CLUMP)
            {
                return true;
            }
            return false;
        }
    }
}
