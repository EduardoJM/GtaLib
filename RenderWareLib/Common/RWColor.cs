using System.IO;

namespace RenderWareLib.Common
{
    public class RWColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public RWColor()
        {
            R = G = B = A = 0;
        }

        public RWColor(byte _r, byte _g, byte _b, byte _a)
        {
            R = _r;
            G = _g;
            B = _b;
            A = _a;
        }

        public RWColor(BinaryReader br)
        {
            R = br.ReadByte();
            G = br.ReadByte();
            B = br.ReadByte();
            A = br.ReadByte();
        }
    }
}
