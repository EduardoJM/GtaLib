using RenderWareLib.SectionsData.TXD;

namespace GtaLib.TXD.Utils
{
    public interface ITXDConverterCompressor
    {
        byte[] Compress(byte[] data, short width, short height, TXDCompression compression);

        byte[] Decompress(byte[] data, short width, short height, TXDCompression compression);
    }
}
