using System;

namespace RenderWareLib.SectionsData
{
    public class RWTextureDictionaryData : RWSectionData
    {
        public ushort TextureCount { get; set; }
        public ushort DeviceId { get; set; }

        public override void Parse(byte[] rawData)
        {
            TextureCount = BitConverter.ToUInt16(rawData, 0);
            DeviceId = BitConverter.ToUInt16(rawData, 2);
        }
    }
}
