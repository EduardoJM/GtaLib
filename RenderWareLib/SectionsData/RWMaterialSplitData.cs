using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RenderWareLib.SectionsData.DFF;

namespace RenderWareLib.SectionsData
{
    public class RWMaterialSplitDataMesh
    {
        public uint IndicesCount { get; set; }

        public uint MaterialIndex { get; set; }

        public uint[] Indices { get; set; }
    }

    public class RWMaterialSplitData : RWSectionData
    {
        public DFFTriangleMode TriangleMode { get; set; }

        public uint MeshCount { get; set; }

        public uint IndicesCount { get; set; }

        public RWMaterialSplitDataMesh[] Meshes { get; set; }

        public override void Parse(RWSection section)
        {
            byte[] rawData = section.Data;
            TriangleMode = (DFFTriangleMode)BitConverter.ToUInt32(rawData, 0);
            MeshCount = BitConverter.ToUInt32(rawData, 4);
            IndicesCount = BitConverter.ToUInt32(rawData, 8);
            int pos = 12;
            Meshes = new RWMaterialSplitDataMesh[MeshCount];
            for (int i = 0; i < MeshCount; i += 1)
            {
                Meshes[i] = new RWMaterialSplitDataMesh();
                Meshes[i].IndicesCount = BitConverter.ToUInt32(rawData, pos);
                Meshes[i].MaterialIndex = BitConverter.ToUInt32(rawData, pos + 4);
                pos += 8;
                Meshes[i].Indices = new uint[Meshes[i].IndicesCount];
                for (int j = 0; j < Meshes[i].IndicesCount; j += 1)
                {
                    Meshes[i].Indices[j] = BitConverter.ToUInt32(rawData, pos);
                    pos += 4;
                }
            }
        }
    }
}
