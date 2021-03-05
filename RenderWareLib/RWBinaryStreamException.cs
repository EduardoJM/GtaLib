using System;

using RenderWareLib.Utils;

namespace RenderWareLib
{
    public class RWBinaryStreamException : Exception
    {
        public RWBinaryStreamException(params string[] messages)
            : base(ArrayUtils.Join(messages))
        {
        }
    }
}
