namespace GtaLib.TXD
{
    public class TXDTextureMipMapData
    {
        public uint RasterSize { get; set; }

        public byte[] RasterData { get; set; }

        public short Width { get; set; }
        public short Height { get; set; }

        public TXDTextureMipMapData() { }
    }
}
