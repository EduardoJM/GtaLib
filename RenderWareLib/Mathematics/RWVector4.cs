using System.IO;

namespace RenderWareLib.Mathematics
{
    public class RWVector4
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; }

        public RWVector4()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 0;
        }

        public RWVector4(float _x, float _y, float _z, float _w)
        {
            X = _x;
            Y = _y;
            Z = _z;
            W = _w;
        }

        public RWVector4(BinaryReader br)
        {
            X = br.ReadSingle();
            Y = br.ReadSingle();
            Z = br.ReadSingle();
            W = br.ReadSingle();
        }
    }
}
