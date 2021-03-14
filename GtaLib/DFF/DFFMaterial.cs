using System;
using System.Collections.Generic;

using RenderWareLib;
using RenderWareLib.Common;

namespace GtaLib.DFF
{
    public class DFFMaterial
    {
        public RWColor Color { get; set; }

        public DFFGeometry Geometry { get; private set; }

        public List<DFFTexture> Textures { get; private set; } = new List<DFFTexture>();

        public void Reparent(DFFGeometry geom)
        {
            if (Geometry != null && geom != null)
            {
                throw new DFFException("Attempt to reparent a DFFMaterial which still has a parent! Remove it from it's old DFFGeometry parent first.");
            }
            Geometry = geom;
        }

        public DFFTexture GetTexture(int index)
        {
            if (index < 0 || index >= Textures.Count)
            {
                throw new IndexOutOfRangeException("Texture index is out of range.");
            }
            return Textures[index];
        }

        public void RemoveTextures()
        {
            Textures.Clear();
        }

        public void RemoveTexture(DFFTexture texture)
        {
            Textures.Remove(texture);
        }

        public void AddTexture(DFFTexture texture)
        {
            Textures.Add(texture);
        }
    }
}
