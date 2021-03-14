using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RenderWareLib;

namespace GtaLib.DFF
{
    public class DFFGeometryPart
    {
        public uint[] Indices { get; set; }

        public DFFMaterial Material { get; private set; }

        public DFFGeometry Geometry { get; private set; }

        public void Reparent(DFFGeometry geom)
        {
            if (geom != null && Geometry != null)
            {
                throw new DFFException("Attempt to reparent a DFFGeometryPart which still has a parent! Remove it from it's old DFFGeometry parent first.");
            }
            Geometry = geom;
        }

        public void SetMaterial(DFFMaterial m)
        {
            if (Geometry == null)
            {
                throw new DFFException("DFFGeometryPart needs to have a DFFGeometry parent for SetMaterial().");
            }
            if (Geometry.IndexOf(m) == -1)
            {
                throw new DFFException("Attempt to assign a material to a DFFGeometryPart which is not owned by it's DFFGeometry.");
            }
            Material = m;
        }
    }
}
