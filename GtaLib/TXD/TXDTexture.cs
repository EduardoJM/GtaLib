using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

using RenderWareLib;
using RenderWareLib.SectionsData;
using RenderWareLib.SectionsData.TXD;

namespace GtaLib.TXD
{
    public class TXDTexture
    {
        private RWTextureNativeData _native = null;

        public string DiffuseName
        {
            get { return _native.DiffuseName; }
            set { _native.DiffuseName = value; }
        }

        public TXDWrappingMode UWrap { get; set; }

        public TXDWrappingMode VWrap { get; set; }

        public TXDFilterFlags FilterFlags { get; set; }

        public TXDCompression Compression { get; set; }

        public byte MipMapCount
        {
            get { return _native.MipmapCount; }
            set { _native.MipmapCount = value; }
        }
        public short Width
        {
            get { return _native.Width; }
            set { _native.Width = value; }
        }
        public short Height
        {
            get { return _native.Height; }
            set { _native.Height = value; }
        }

        public TXDRasterFormat RasterFormat
        {
            get { return (TXDRasterFormat)_native.RasterFormat; }
            set { _native.RasterFormat = (int)value; }
        }

        public byte[] Palette { get; private set; }

        internal TXDTexture(RWTextureNativeData native)
        {
            _native = native;
            UWrap = (TXDWrappingMode)native.UWrap;
            VWrap = (TXDWrappingMode)native.VWrap;
            FilterFlags = (TXDFilterFlags)native.FilterFlags;
            Compression = native.GetTXDCompressionMethod();
        }

        public TXDRasterFormat GetRasterFormatExtension()
        {
            return (TXDRasterFormat)(_native.RasterFormat & (int)TXDRasterFormat.RasterFormatEXTMask);
        }

        public bool IsAlphaChannelUsed()
        {
            return _native.GetAlphaIsUsed();
        }

        public TXDTextureMipMapData[] GetMipLevelsData()
        {
            int position = 0;
            if ((GetRasterFormatExtension() & TXDRasterFormat.RasterFormatEXTPAL4) != 0)
            {
                Palette = new byte[16 * 4];
                Array.Copy(_native.RawData, 0, Palette, 0, 16 * 4);
                position += 16 * 4;
            }
            else if ((GetRasterFormatExtension() & TXDRasterFormat.RasterFormatEXTPAL8) != 0)
            {
                Palette = new byte[256 * 4];
                Array.Copy(_native.RawData, 0, Palette, 0, 256 * 4);
                position += 256 * 4;
            }
            List<TXDTextureMipMapData> data = new List<TXDTextureMipMapData>();
            bool mipmapsIncluded = (GetRasterFormatExtension() & TXDRasterFormat.RasterFormatEXTMipmap) != 0;
            int numIncludedMipmaps = mipmapsIncluded ? MipMapCount : 1;
            short mipW = Width;
            short mipH = Height;
            for (int i = 0; i < numIncludedMipmaps; i += 1)
            {
                if (mipW < 4 || mipH < 4)
                {
                    break;
                }
                TXDTextureMipMapData dt = new TXDTextureMipMapData();
                dt.RasterSize = BitConverter.ToUInt32(_native.RawData, position);
                dt.RasterData = new byte[dt.RasterSize];
                Array.Copy(_native.RawData, position + 4, dt.RasterData, 0, dt.RasterSize);
                dt.Width = mipW;
                dt.Height = mipH;
                position += (int)dt.RasterSize + 4;
                data.Add(dt);
                mipW /= 2;
                mipH /= 2;
            }
            return data.ToArray();
        }

        public TXDTexture Clone()
        {
            return new TXDTexture(_native.Clone());
        }
    }
}
