using System.IO;

namespace RenderWareLib.Mathematics
{
    public class RWVector3
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public RWVector3()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public RWVector3(float _x, float _y, float _z)
        {
            X = _x;
            Y = _y;
            Z = _z;
        }

        public RWVector3(BinaryReader br)
        {
            X = br.ReadSingle();
            Y = br.ReadSingle();
            Z = br.ReadSingle();
        }
    }
}
