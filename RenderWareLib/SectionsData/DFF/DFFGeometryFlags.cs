using System;

namespace RenderWareLib.SectionsData.DFF
{
    [Flags]
    public enum DFFGeometryFlags : ushort
    {
        GEOMETRY_FLAG_TRISTRIP = (1 << 0),
        GEOMETRY_FLAG_POSITIONS = (1 << 1),
        GEOMETRY_FLAG_TEXCOORDS = (1 << 2),
        GEOMETRY_FLAG_COLORS = (1 << 3),
        GEOMETRY_FLAG_NORMALS = (1 << 4),
        GEOMETRY_FLAG_DYNAMICLIGHTING = (1 << 5),
        GEOMETRY_FLAG_MODULATEMATCOLOR = (1 << 6),
        GEOMETRY_FLAG_MULTIPLEUVSETS = (1 << 7)
    }
}
