using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RenderWareLib.Mathematics;
using OpenTK.Mathematics;

namespace GtaLib.Renderer
{
    public static class MathematicsExtensions
    {
        public static Vector4 ToGL(this RWVector4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Matrix4 ToGL(this RWMatrix4 m)
        {
            return new Matrix4(m.Row1.ToGL(), m.Row2.ToGL(), m.Row3.ToGL(), m.Row4.ToGL());
        }
    }
}
