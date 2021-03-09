using System;

namespace RenderWareLib.SectionsData
{
    public class RWTextureDictionaryData : RWSectionData
    {
        public ushort TextureCount { get; set; }
        public ushort DeviceId { get; set; }

        public override void Parse(RWSection section)
        {
            byte[] rawData = section.Data;
            TextureCount = BitConverter.ToUInt16(rawData, 0);
            DeviceId = BitConverter.ToUInt16(rawData, 2);
        }
    }
}
