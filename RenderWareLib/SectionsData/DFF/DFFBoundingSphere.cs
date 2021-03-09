namespace RenderWareLib.SectionsData.DFF
{
    public class DFFBoundingSphere
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float Radius { get; set; }

        public DFFBoundingSphere()
        {
            X = Y = Z = 0;
            Radius = 1;
        }

        public DFFBoundingSphere(float _x, float _y, float _z, float _radius)
        {
            X = _x;
            Y = _y;
            Z = _z;
            Radius = _radius;
        }
    }
}
