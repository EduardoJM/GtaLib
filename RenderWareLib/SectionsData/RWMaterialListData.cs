using System;

namespace RenderWareLib.SectionsData
{
    public class RWMaterialListData: RWSectionData
    {
        public uint MaterialCount { get; set; }

        public uint[] Indices { get; set; }

        public override void Parse(RWSection section)
        {
            byte[] rawData = section.Data;
            MaterialCount = BitConverter.ToUInt32(rawData, 0);
            Indices = new uint[MaterialCount];
            for (int i = 0; i < MaterialCount; i += 1)
            {
                Indices[i] = BitConverter.ToUInt32(rawData, 4 + i * 4);
            }
        }

    }
}
