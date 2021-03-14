using System;
using System.IO;

namespace RenderWareLib.Mathematics
{
    public class RWVector4
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; }

        public static readonly RWVector4 UnitX = new RWVector4(1, 0, 0, 0);

        public static readonly RWVector4 UnitY = new RWVector4(0, 1, 0, 0);

        public static readonly RWVector4 UnitZ = new RWVector4(0, 0, 1, 0);

        public static readonly RWVector4 UnitW = new RWVector4(0, 0, 0, 1);

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

        public RWVector4(RWVector4 other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
            W = other.W;
        }

        public static bool operator ==(RWVector4 left, RWVector4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RWVector4 left, RWVector4 right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is RWVector4 && Equals((RWVector4)obj);
        }

        public bool Equals(RWVector4 other)
        {
            return
                X == X &&
                Y == Y &&
                Z == Z &&
                W == W;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z, W);
        }
    }
}
