using System;

namespace RenderWareLib.SectionsData
{
    public class RWGeometryListData : RWSectionData
    {
        public uint GeometryCount { get; set; }

        public override void Parse(RWSection section)
        {
            byte[] rawData = section.Data;
            GeometryCount = BitConverter.ToUInt32(rawData, 0);
        }
    }
}
