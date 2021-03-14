using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RenderWareLib.Mathematics;

namespace GtaLib.DFF
{
    public class DFFMesh
    {
        public int BoneCount { get; set; }

        public DFFFrame RootFrame { get; set; } = new DFFFrame(RWMatrix4.Identity);

        public List<DFFGeometry> Geometries { get; private set; } = new List<DFFGeometry>();

        public void AddGeometry(DFFGeometry geom)
        {
            geom.Reparent(this);
            Geometries.Add(geom);
        }

        public DFFGeometry GetGeometry(int index)
        {
            if (index < 0 || index >= Geometries.Count)
            {
                throw new IndexOutOfRangeException("Geometry index is out of range.");
            }
            return Geometries[index];
        }
    }
}
