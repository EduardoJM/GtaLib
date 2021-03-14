using System;

namespace RenderWareLib.SectionsData
{
    public class RWAtomicData : RWSectionData
    {
        public uint FrameIndex { get; set; }

        public uint GeometryIndex { get; set; }

        public uint Unknown1 { get; set; }

        public uint Unknown2 { get; set; }

        public override void Parse(RWSection section)
        {
            byte[] rawData = section.Data;
            FrameIndex = BitConverter.ToUInt32(rawData, 0);
            GeometryIndex = BitConverter.ToUInt32(rawData, 4);
            Unknown1 = BitConverter.ToUInt32(rawData, 8);
            Unknown2 = BitConverter.ToUInt32(rawData, 12);
        }
    }
}
