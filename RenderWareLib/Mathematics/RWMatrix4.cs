using System.IO;

namespace RenderWareLib.Mathematics
{
    public class RWMatrix4
    {
        public RWVector4 Row1 { get; set; }

        public RWVector4 Row2 { get; set; }

        public RWVector4 Row3 { get; set; }

        public RWVector4 Row4 { get; set; }

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
    }
}
