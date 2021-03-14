using System;

namespace RenderWareLib.SectionsData
{
    public class RWTextureData : RWSectionData
    {
        public ushort FilterFlags { get; set; }

        public ushort Unknown { get; set; }

        public override void Parse(RWSection section)
        {
            byte[] rawData = section.Data;
            FilterFlags = BitConverter.ToUInt16(rawData, 0);
            Unknown = BitConverter.ToUInt16(rawData, 2);
        }
    }
}
