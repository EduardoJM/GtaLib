using System.IO;

namespace RenderWareLib.Mathematics
{
    public class RWVector2
    {
        public float X { get; set; }

        public float Y { get; set; }

        public RWVector2()
        {
            X = 0;
            Y = 0;
        }

        public RWVector2(float _x, float _y)
        {
            X = _x;
            Y = _y;
        }

        public RWVector2(BinaryReader br)
        {
            X = br.ReadSingle();
            Y = br.ReadSingle();
        }
    }
}
