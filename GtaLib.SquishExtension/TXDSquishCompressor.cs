using GtaLib.TXD.Utils;
using RenderWareLib.SectionsData.TXD;

namespace GtaLib.Squish
{
    public class TXDSquishCompressor : ITXDConverterCompressor
    {
        public byte[] Compress(byte[] data, short width, short height, TXDCompression compression)
        {
            return Squish.CompressImage(
                data,
                width,
                height,
                compression == TXDCompression.DXT1 ? SquishFlags.Dxt1 : SquishFlags.Dxt3
            );
        }

        public byte[] Decompress(byte[] data, short width, short height, TXDCompression compression)
        {
            return Squish.DecompressImage(
                data,
                width,
                height,
                compression == TXDCompression.DXT1 ? SquishFlags.Dxt1 : SquishFlags.Dxt3
            );
        }
    }
}
