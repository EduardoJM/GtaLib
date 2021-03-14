using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RenderWareLib;
using RenderWareLib.Common;
using RenderWareLib.Mathematics;
using RenderWareLib.SectionsData.DFF;

namespace GtaLib.DFF
{
    public class DFFGeometry
    {
        public RWVector3[] Vertices { get; set; }

        public RWVector3[] Normals { get; set; }

        public DFFUvSet[] UVSets { get; set; }

        public RWColor[] VertexColors { get; set; }

        public DFFGeometryFlags Flags { get; set; }

        public DFFBoundingSphere Bounds { get; set; }

        public float AmbientLight { get; set; } = 0.0f;

        public float DiffuseLight { get; set; } = 0.0f;

        public float SpecularLight { get; set; } = 0.0f;

        public List<DFFMaterial> Materials { get; private set; } = new List<DFFMaterial>();

        public DFFFrame AssociatedFrame { get; set; }

        public List<DFFGeometryPart> Parts { get; private set; } = new List<DFFGeometryPart>();

        public DFFMesh Mesh { get; private set; }

        public DFFGeometry()
        {
        }

        public bool IsTriangleStripFormat()
        {
            return (Flags & DFFGeometryFlags.GEOMETRY_FLAG_TRISTRIP) != 0;
        }

        public bool IsDynamicLightingEnabled()
        {
            return (Flags & DFFGeometryFlags.GEOMETRY_FLAG_DYNAMICLIGHTING) != 0;
        }

        public int IndexOf(DFFMaterial material)
        {
            return Materials.IndexOf(material);
        }

        public DFFMaterial GetMaterial(int index)
        {
            if (index < 0 || index >= Materials.Count)
            {
                throw new IndexOutOfRangeException("Geometry materials index is out of range.");
            }
            return Materials[index];
        }

        public void RemoveMaterials()
        {
            for (int i = 0; i < Materials.Count; i += 1)
            {
                Materials[i].Reparent(null);
            }
            Materials.Clear();
        }

        public void RemoveMaterial(DFFMaterial mat)
        {
            mat.Reparent(null);
            Materials.Remove(mat);
        }

        public void AddMaterial(DFFMaterial mat)
        {
            mat.Reparent(this);
            Materials.Add(mat);
        }

        public void Reparent(DFFMesh mesh)
        {
            if (Mesh != null && mesh != null)
            {
                throw new DFFException("Attempt to reparent a DFFGeometry which still has a parent! Remove it from it's old DFFMesh parent first.");
            }
            Mesh = mesh;
        }

        public DFFGeometryPart GetPart(int index)
        {
            if (index < 0 || index >= Parts.Count)
            {
                throw new IndexOutOfRangeException("Geometry parts index is out of range.");
            }
            return Parts[index];
        }

        public void RemoveParts()
        {
            for (int i = 0; i < Parts.Count; i += 1)
            {
                Parts[i].Reparent(null);
            }
            Parts.Clear();
        }

        public void RemovePart(DFFGeometryPart part)
        {
            part.Reparent(null);
            Parts.Remove(part);
        }

        public void AddPart(DFFGeometryPart part)
        {
            part.Reparent(this);
            Parts.Add(part);
        }
    }
}
