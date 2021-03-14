using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RenderWareLib.Common;

namespace RenderWareLib.SectionsData
{
    public class RWMaterialData: RWSectionData
    {
        public int Flags { get; set; }

        public RWColor Color { get; set; }

        public int Unused { get; set; }

        public int TextureCount { get; set; }

        public float Ambient { get; set; }
    
        public float Specular { get; set; }
    
        public float Diffuse { get; set; }

        public override void Parse(RWSection section)
        {
            byte[] rawData = section.Data;
            Flags = BitConverter.ToInt32(rawData, 0);
            Color = new RWColor(rawData[4], rawData[5], rawData[6], rawData[7]);
            Unused = BitConverter.ToInt32(rawData, 8);
            TextureCount = BitConverter.ToInt32(rawData, 12);
            if (section.Header.Version > RWVersion.RW_VERSION_GTAVC_2)
            {
                Ambient = BitConverter.ToSingle(rawData, 16);
                Diffuse = BitConverter.ToSingle(rawData, 20);
                Specular = BitConverter.ToSingle(rawData, 24);
            }
        }
    }
}
