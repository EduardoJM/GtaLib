using System;
using System.Collections.Generic;

using RenderWareLib.Common;
using RenderWareLib.Mathematics;
using RenderWareLib.SectionsData.DFF;

namespace RenderWareLib.SectionsData
{
    public class RWGeometryDataTriangle
    {
        public ushort Vertex2 { get; set; }

        public ushort Vertex1 { get; set; }

        public ushort MaterialId { get; set; }

        public ushort Vertex3 { get; set; }
    }

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

        public RWGeometryDataTriangle[] Triangles { get; set; }

        public byte[] GetData(RWSection section)
        {
            List<byte> output = new List<byte>();
            output.AddRange(BitConverter.GetBytes((ushort)Flags));
            output.Add(UvSetCount);
            output.Add(0);
            output.AddRange(BitConverter.GetBytes(TriangleCount));
            output.AddRange(BitConverter.GetBytes(VertexCount));
            output.AddRange(BitConverter.GetBytes(FrameCount));

            if (section.Header.Version <= RWVersion.RW_VERSION_GTAVC_2)
            {
                output.AddRange(BitConverter.GetBytes(AmbientColor));
                output.AddRange(BitConverter.GetBytes(DiffuseColor));
                output.AddRange(BitConverter.GetBytes(SpecularColor));
            }

            if ((Flags & DFFGeometryFlags.GEOMETRY_FLAG_COLORS) != 0)
            {
                for (int i = 0; i < VertexColors.Length; i += 1)
                {
                    output.Add(VertexColors[i].R);
                    output.Add(VertexColors[i].G);
                    output.Add(VertexColors[i].B);
                    output.Add(VertexColors[i].A);
                }
            }

            if ((Flags & (DFFGeometryFlags.GEOMETRY_FLAG_TEXCOORDS | DFFGeometryFlags.GEOMETRY_FLAG_MULTIPLEUVSETS)) != 0)
            {
                for (int i = 0; i < UvSetCount; i += 1)
                {
                    for (int j = 0; j < TexCoords[i].Length; j += 1)
                    {
                        output.AddRange(BitConverter.GetBytes(TexCoords[i][j].X));
                        output.AddRange(BitConverter.GetBytes(TexCoords[i][j].Y));
                    }
                }
            }

            for (int i = 0; i < TriangleCount; i += 1)
            {
                output.AddRange(BitConverter.GetBytes(Triangles[i].Vertex2));
                output.AddRange(BitConverter.GetBytes(Triangles[i].Vertex1));
                output.AddRange(BitConverter.GetBytes(Triangles[i].MaterialId));
                output.AddRange(BitConverter.GetBytes(Triangles[i].Vertex3));
            }
            
            output.AddRange(BitConverter.GetBytes(Bounds.X));
            output.AddRange(BitConverter.GetBytes(Bounds.Y));
            output.AddRange(BitConverter.GetBytes(Bounds.Z));
            output.AddRange(BitConverter.GetBytes(Bounds.Radius));
            
            output.AddRange(BitConverter.GetBytes((int)1));
            output.AddRange(BitConverter.GetBytes((int)((Flags & DFFGeometryFlags.GEOMETRY_FLAG_NORMALS) != 0 ? 1 : 0)));

            for (int i = 0; i < Vertices.Length; i += 1)
            {
                output.AddRange(BitConverter.GetBytes(Vertices[i].X));
                output.AddRange(BitConverter.GetBytes(Vertices[i].Y));
                output.AddRange(BitConverter.GetBytes(Vertices[i].Z));
            }

            if ((Flags & DFFGeometryFlags.GEOMETRY_FLAG_NORMALS) != 0)
            {
                for (int i = 0; i < Normals.Length; i += 1)
                {
                    output.AddRange(BitConverter.GetBytes(Normals[i].X));
                    output.AddRange(BitConverter.GetBytes(Normals[i].Y));
                    output.AddRange(BitConverter.GetBytes(Normals[i].Z));
                }
            }

            return output.ToArray();
        }

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
                    for (int j = 0; j < VertexCount; j += 1)
                    {
                        TexCoords[i][j] = new RWVector2(
                            BitConverter.ToSingle(rawData, geomDataPosition),
                            BitConverter.ToSingle(rawData, geomDataPosition + 4)
                        );
                        geomDataPosition += 8;
                    }
                }
            }
            Triangles = new RWGeometryDataTriangle[TriangleCount];
            for (int i = 0; i < TriangleCount; i += 1)
            {
                Triangles[i] = new RWGeometryDataTriangle();
                Triangles[i].Vertex2 = BitConverter.ToUInt16(rawData, geomDataPosition);
                Triangles[i].Vertex1 = BitConverter.ToUInt16(rawData, geomDataPosition + 2);
                Triangles[i].MaterialId = BitConverter.ToUInt16(rawData, geomDataPosition + 4);
                Triangles[i].Vertex3 = BitConverter.ToUInt16(rawData, geomDataPosition + 6);
                geomDataPosition += 8;
            }
            // Skip Triangles Here...
            // geomDataPosition += (int)TriangleCount * 8;
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
