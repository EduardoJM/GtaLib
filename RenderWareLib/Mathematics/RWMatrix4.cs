using System;
using System.IO;

namespace RenderWareLib.Mathematics
{
    public class RWMatrix4
    {
        public RWVector4 Row1 { get; set; }

        public RWVector4 Row2 { get; set; }

        public RWVector4 Row3 { get; set; }

        public RWVector4 Row4 { get; set; }

        public static readonly RWMatrix4 Identity = new RWMatrix4(RWVector4.UnitX, RWVector4.UnitY, RWVector4.UnitZ, RWVector4.UnitW);

        public RWMatrix4()
        {
            Row1 = new RWVector4();
            Row2 = new RWVector4();
            Row3 = new RWVector4();
            Row4 = new RWVector4();
        }

        public RWMatrix4(
            float a_11, float a_12, float a_13, float a_14,
            float a_21, float a_22, float a_23, float a_24,
            float a_31, float a_32, float a_33, float a_34,
            float a_41, float a_42, float a_43, float a_44)
        {
            Row1 = new RWVector4(a_11, a_12, a_13, a_14);
            Row2 = new RWVector4(a_21, a_22, a_23, a_24);
            Row3 = new RWVector4(a_31, a_32, a_33, a_34);
            Row4 = new RWVector4(a_41, a_42, a_43, a_44);
        }

        public RWMatrix4(BinaryReader br)
        {
            Row1 = new RWVector4(br);
            Row2 = new RWVector4(br);
            Row3 = new RWVector4(br);
            Row4 = new RWVector4(br);
        }

        public RWMatrix4(RWVector4 _row1, RWVector4 _row2, RWVector4 _row3, RWVector4 _row4)
        {
            Row1 = new RWVector4(_row1);
            Row2 = new RWVector4(_row2);
            Row3 = new RWVector4(_row3);
            Row4 = new RWVector4(_row4);
        }

        public RWMatrix4(RWMatrix4 other)
        {
            Row1 = new RWVector4(other.Row1);
            Row2 = new RWVector4(other.Row2);
            Row3 = new RWVector4(other.Row3);
            Row4 = new RWVector4(other.Row4);
        }

        public static void Mult(in RWMatrix4 left, in RWMatrix4 right, out RWMatrix4 result)
        {
            float leftM11 = left.Row1.X;
            float leftM12 = left.Row1.Y;
            float leftM13 = left.Row1.Z;
            float leftM14 = left.Row1.W;
            float leftM21 = left.Row2.X;
            float leftM22 = left.Row2.Y;
            float leftM23 = left.Row2.Z;
            float leftM24 = left.Row2.W;
            float leftM31 = left.Row3.X;
            float leftM32 = left.Row3.Y;
            float leftM33 = left.Row3.Z;
            float leftM34 = left.Row3.W;
            float leftM41 = left.Row4.X;
            float leftM42 = left.Row4.Y;
            float leftM43 = left.Row4.Z;
            float leftM44 = left.Row4.W;
            float rightM11 = right.Row1.X;
            float rightM12 = right.Row1.Y;
            float rightM13 = right.Row1.Z;
            float rightM14 = right.Row1.W;
            float rightM21 = right.Row2.X;
            float rightM22 = right.Row2.Y;
            float rightM23 = right.Row2.Z;
            float rightM24 = right.Row2.W;
            float rightM31 = right.Row3.X;
            float rightM32 = right.Row3.Y;
            float rightM33 = right.Row3.Z;
            float rightM34 = right.Row3.W;
            float rightM41 = right.Row4.X;
            float rightM42 = right.Row4.Y;
            float rightM43 = right.Row4.Z;
            float rightM44 = right.Row4.W;

            result = new RWMatrix4();
            result.Row1.X = (leftM11 * rightM11) + (leftM12 * rightM21) + (leftM13 * rightM31) + (leftM14 * rightM41);
            result.Row1.Y = (leftM11 * rightM12) + (leftM12 * rightM22) + (leftM13 * rightM32) + (leftM14 * rightM42);
            result.Row1.Z = (leftM11 * rightM13) + (leftM12 * rightM23) + (leftM13 * rightM33) + (leftM14 * rightM43);
            result.Row1.W = (leftM11 * rightM14) + (leftM12 * rightM24) + (leftM13 * rightM34) + (leftM14 * rightM44);
            result.Row2.X = (leftM21 * rightM11) + (leftM22 * rightM21) + (leftM23 * rightM31) + (leftM24 * rightM41);
            result.Row2.Y = (leftM21 * rightM12) + (leftM22 * rightM22) + (leftM23 * rightM32) + (leftM24 * rightM42);
            result.Row2.Z = (leftM21 * rightM13) + (leftM22 * rightM23) + (leftM23 * rightM33) + (leftM24 * rightM43);
            result.Row2.W = (leftM21 * rightM14) + (leftM22 * rightM24) + (leftM23 * rightM34) + (leftM24 * rightM44);
            result.Row3.X = (leftM31 * rightM11) + (leftM32 * rightM21) + (leftM33 * rightM31) + (leftM34 * rightM41);
            result.Row3.Y = (leftM31 * rightM12) + (leftM32 * rightM22) + (leftM33 * rightM32) + (leftM34 * rightM42);
            result.Row3.Z = (leftM31 * rightM13) + (leftM32 * rightM23) + (leftM33 * rightM33) + (leftM34 * rightM43);
            result.Row3.W = (leftM31 * rightM14) + (leftM32 * rightM24) + (leftM33 * rightM34) + (leftM34 * rightM44);
            result.Row4.X = (leftM41 * rightM11) + (leftM42 * rightM21) + (leftM43 * rightM31) + (leftM44 * rightM41);
            result.Row4.Y = (leftM41 * rightM12) + (leftM42 * rightM22) + (leftM43 * rightM32) + (leftM44 * rightM42);
            result.Row4.Z = (leftM41 * rightM13) + (leftM42 * rightM23) + (leftM43 * rightM33) + (leftM44 * rightM43);
            result.Row4.W = (leftM41 * rightM14) + (leftM42 * rightM24) + (leftM43 * rightM34) + (leftM44 * rightM44);
        }

        public static RWMatrix4 Mult(RWMatrix4 left, RWMatrix4 right)
        {
            Mult(in left, in right, out RWMatrix4 result);
            return result;
        }

        public static RWMatrix4 operator *(RWMatrix4 left, RWMatrix4 right)
        {
            return Mult(left, right);
        }

        public static bool operator ==(RWMatrix4 left, RWMatrix4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RWMatrix4 left, RWMatrix4 right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is RWMatrix4 && Equals((RWMatrix4)obj);
        }

        public bool Equals(RWMatrix4 other)
        {
            return
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3 &&
                Row4 == other.Row4;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row1, Row2, Row3, Row4);
        }
    }
}
