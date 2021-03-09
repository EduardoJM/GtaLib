using System;

using RenderWareLib.Common;
using RenderWareLib.Mathematics;
using RenderWareLib.SectionsData.DFF;

namespace RenderWareLib.SectionsData
{
    public class RWGeometryData : RWSectionData
    {
        public DFFGeometryFlags Flags { get; set; }

        public byte UvSetCount { get; set; }

        public uint TriangleCount { get; set; }

        public uint VertexCount { get; set; }

        public uint FrameCount { get; set; }

        public float AmbientColor { get; set; }

        public float DiffuseColor { get; set; }

        public float SpecularColor { get; set; }

        public RWColor[] VertexColors { get; set; }

        public RWVector2[][] TexCoords { get; set; }

        public DFFBoundingSphere Bounds { get; set; }

        public RWVector3[] Vertices { get; set; }
        public RWVector3[] Normals { get; set; }

        public override void Parse(RWSection section)
        {
            byte[] rawData = section.Data;
            Flags = (DFFGeometryFlags)BitConverter.ToUInt16(rawData, 0);
            UvSetCount = rawData[2];
            TriangleCount = BitConverter.ToUInt32(rawData, 4);
            VertexCount = BitConverter.ToUInt32(rawData, 8);
            FrameCount = BitConverter.ToUInt32(rawData, 12);

            if ((Flags & DFFGeometryFlags.GEOMETRY_FLAG_TEXCOORDS) != 0 &&
                (Flags & DFFGeometryFlags.GEOMETRY_FLAG_MULTIPLEUVSETS) == 0)
            {
                UvSetCount = 1;
            }

            AmbientColor = 0.0f;
            DiffuseColor = 0.0f;
            SpecularColor = 0.0f;

            int geomDataPosition = 16;
            if (section.Header.Version <= RWVersion.RW_VERSION_GTAVC_2)
            {
                AmbientColor = BitConverter.ToSingle(rawData, geomDataPosition);
                DiffuseColor = BitConverter.ToSingle(rawData, geomDataPosition + 4);
                SpecularColor = BitConverter.ToSingle(rawData, geomDataPosition + 8);
                geomDataPosition += 12;
            }

            if ((Flags & DFFGeometryFlags.GEOMETRY_FLAG_COLORS) != 0)
            {
                VertexColors = new RWColor[VertexCount];
                for (int i = 0; i < VertexColors.Length; i += 1)
                {
                    VertexColors[i] = new RWColor(
                        rawData[geomDataPosition],
                        rawData[geomDataPosition + 1],
                        rawData[geomDataPosition + 2],
                        rawData[geomDataPosition + 3]
                    );
                    geomDataPosition += 4;
                }
            }

            if ((Flags & (DFFGeometryFlags.GEOMETRY_FLAG_TEXCOORDS | DFFGeometryFlags.GEOMETRY_FLAG_MULTIPLEUVSETS)) != 0)
            {
                TexCoords = new RWVector2[UvSetCount][];
                for (int i = 0; i < UvSetCount; i += 1)
                {
                    TexCoords[i] = new RWVector2[VertexCount];
                    for (int j = 0; j < TexCoords[i].Length; j += i)
                    {
                        TexCoords[i][j] = new RWVector2(
                            BitConverter.ToSingle(rawData, geomDataPosition),
                            BitConverter.ToSingle(rawData, geomDataPosition + 4)
                        );
                        geomDataPosition += 8;
                    }
                }
                // Skip Triangles Here...
                geomDataPosition += (int)TriangleCount * 8;
                //
                Bounds = new DFFBoundingSphere(
                    BitConverter.ToSingle(rawData, geomDataPosition),
                    BitConverter.ToSingle(rawData, geomDataPosition + 4),
                    BitConverter.ToSingle(rawData, geomDataPosition + 8),
                    BitConverter.ToSingle(rawData, geomDataPosition + 12)
                );
                geomDataPosition += 16;
                geomDataPosition += 8;
                Vertices = new RWVector3[VertexCount];
                for (int i = 0; i < Vertices.Length; i += 1)
                {
                    Vertices[i] = new RWVector3(
                        BitConverter.ToSingle(rawData, geomDataPosition),
                        BitConverter.ToSingle(rawData, geomDataPosition + 4),
                        BitConverter.ToSingle(rawData, geomDataPosition + 8)
                    );
                    geomDataPosition += 12;
                }

                if ((Flags & DFFGeometryFlags.GEOMETRY_FLAG_NORMALS) != 0)
                {
                    Normals = new RWVector3[VertexCount];
                    for (int i = 0; i < Normals.Length; i += 1)
                    {
                        Normals[i] = new RWVector3(
                            BitConverter.ToSingle(rawData, geomDataPosition),
                            BitConverter.ToSingle(rawData, geomDataPosition + 4),
                            BitConverter.ToSingle(rawData, geomDataPosition + 8)
                        );
                        geomDataPosition += 12;
                    }
                }
            }
        }
    }
}
