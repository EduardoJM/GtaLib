using System.IO;

namespace RenderWareLib.Mathematics
{
    public class RWMatrix3
    {
        public RWVector3 Row1 { get; set; }

        public RWVector3 Row2 { get; set; }

        public RWVector3 Row3 { get; set; }

        public RWMatrix3()
        {
            Row1 = new RWVector3();
            Row2 = new RWVector3();
            Row3 = new RWVector3();
        }

        public RWMatrix3(float a_11, float a_12, float a_13, float a_21, float a_22, float a_23, float a_31, float a_32, float a_33)
        {
            Row1 = new RWVector3(a_11, a_12, a_13);
            Row2 = new RWVector3(a_21, a_22, a_23);
            Row3 = new RWVector3(a_31, a_32, a_33);
        }

        public RWMatrix3(BinaryReader br)
        {
            Row1 = new RWVector3(br);
            Row2 = new RWVector3(br);
            Row3 = new RWVector3(br);
        }
    }
}
