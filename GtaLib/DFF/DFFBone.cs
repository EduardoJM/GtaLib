using RenderWareLib.Mathematics;

namespace GtaLib.DFF
{
    public class DFFBone
    {
        public DFFBoneType Type { get; set; }

        public RWMatrix4 InverseBoneMatrix { get; set; }

        public uint Number { get; set; }

        public uint Index { get; set; }

        public DFFBone()
        {
        }

        public DFFBone(DFFBone other)
        {
            Index = other.Index;
            Number = other.Number;
            Type = other.Type;
        }
    }
}
