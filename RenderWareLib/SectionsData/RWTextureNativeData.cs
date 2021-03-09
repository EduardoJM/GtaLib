using System;
using System.Text;

using RenderWareLib.SectionsData.TXD;

namespace RenderWareLib.SectionsData
{
    public class RWTextureNativeData : RWSectionData
    {
		public int Platform { get; set; }
		public short FilterFlags { get; set; }
		public byte VWrap { get; set; }
		public byte UWrap { get; set; }
		public string DiffuseName { get; set; }
		public string AlphaName { get; set; }
		public int RasterFormat { get; set; }
		public int AlphaOrCompression { get; set; }
		public short Width { get; set; }
		public short Height { get; set; }
		public byte Bpp { get; set; }
		public byte MipmapCount { get; set; }
		public byte RasterType { get; set; }
		public byte CompressionOrAlpha { get; set; }

		public byte[] RawData { get; set; }

        public override void Parse(RWSection section)
        {
			byte[] rawData = section.Data;
			Platform = BitConverter.ToInt32(rawData, 0);
			FilterFlags = BitConverter.ToInt16(rawData, 4);
			VWrap = rawData[6];
			UWrap = rawData[7];
			int numRead = 8;
			DiffuseName = Encoding.ASCII.GetString(rawData, numRead, 32).Split(new Char[1])[0];
			AlphaName = Encoding.ASCII.GetString(rawData, numRead + 32, 32).Split(new Char[1])[0];
			numRead += 64;
			RasterFormat = BitConverter.ToInt32(rawData, numRead);
			AlphaOrCompression = BitConverter.ToInt32(rawData, numRead + 4);
			Width = BitConverter.ToInt16(rawData, numRead + 8);
			Height = BitConverter.ToInt16(rawData, numRead + 10);
			numRead += 12;
			Bpp = rawData[numRead];
			MipmapCount = rawData[numRead + 1];
			RasterType = rawData[numRead + 2];
			CompressionOrAlpha = rawData[numRead + 3];
			numRead += 4;
			RawData = new byte[rawData.Length - numRead];
			Array.Copy(rawData, numRead, RawData, 0, RawData.Length);
		}

		public TXDCompression GetTXDCompressionMethod()
		{
			TXDCompression method = TXDCompression.NONE;
			if (Platform == 0x325350)
			{
				throw new Exception("PS2 format of TXD is currently not supported!");
			}
			else if (Platform == 9)
            {
				string fourCC = Encoding.ASCII.GetString(BitConverter.GetBytes(AlphaOrCompression));
				if (fourCC[0] == 'D' && fourCC[1] == 'X' && fourCC[2] == 'T')
				{
					if (fourCC[3] == '1')
					{
						method = TXDCompression.DXT1;
					}
					else if (fourCC[3] == '3')
					{
						method = TXDCompression.DXT3;
					}
					else if (fourCC[3] == '5')
                    {
						method = TXDCompression.DXT5;
					}
				}
			}
			else
            {
				if (CompressionOrAlpha == 1)
				{
					method = TXDCompression.DXT1;
				}
				else if (CompressionOrAlpha == 3)
				{
					method = TXDCompression.DXT3;
				}
			}
			return method;
		}

		public bool GetAlphaIsUsed()
        {
			if (Platform == 0x325350)
			{
				throw new Exception("PS2 format of TXD is currently not supported!");
			}
			else if (Platform == 9)
            {
				return (CompressionOrAlpha == 9 || CompressionOrAlpha == 1);
			}
			else
            {
				return AlphaOrCompression == 1;
            }
		}

		public TXDRasterFormat GetRasterFormat(TXDCompression compression)
		{
			if (RasterFormat == (int)TXDRasterFormat.RasterFormatDefault)
			{
				switch (compression)
				{
					case TXDCompression.DXT1:
						return TXDRasterFormat.RasterFormatR5G6B5;
					case TXDCompression.DXT3:
						return TXDRasterFormat.RasterFormatR4G4B4A4;
					case TXDCompression.NONE:
						throw new TXDException("Raster format DEFAULT is invalid for uncompressed textures!");
				}
			}
			if (CalculateMaximumMipmapLevel(compression) < 0)
			{
				throw new TXDException("Invalid texture dimensions for this format!");
			}
			return (TXDRasterFormat)RasterFormat;
		}

		public int CalculateMaximumMipmapLevel(TXDCompression compression)
		{
			int minW, minH;

			switch (compression)
			{
				case TXDCompression.DXT1:
				case TXDCompression.DXT3:
				case TXDCompression.DXT5:
					minW = 4;
					minH = 4;
					break;
				default:
					minW = 1;
					minH = 1;
					break;
			}
			return (int)Math.Min(-(Math.Log10((float)minW / Width) / Math.Log10(2.0f)), -(Math.Log10((float)minH / Height) / Math.Log10(2.0f)));
		}

		public int GetBytesPerPixel(TXDCompression compression)
        {
			if ((RasterFormat & (int)TXDRasterFormat.RasterFormatEXTPAL4) != 0 || (RasterFormat & (int)TXDRasterFormat.RasterFormatEXTPAL8) != 0)
			{
				return 1;
			}
			else
			{
				switch (GetRasterFormat(compression))
				{
					case TXDRasterFormat.RasterFormatB8G8R8A8:
					case TXDRasterFormat.RasterFormatR8G8B8A8:
					case TXDRasterFormat.RasterFormatB8G8R8:
						return 4;

					case TXDRasterFormat.RasterFormatA1R5G5B5:
					case TXDRasterFormat.RasterFormatR4G4B4A4:
					case TXDRasterFormat.RasterFormatR5G5B5:
					case TXDRasterFormat.RasterFormatR5G6B5:
						return 2;

					case TXDRasterFormat.RasterFormatLUM8:
						return 1;

					default:
						// TODO What's the default BPP?
						return 0;
				}
			}
		}

		public int ComputeMipmapDataSize(TXDCompression compression, int mipmap)
		{
			int bpp = GetBytesPerPixel(compression);
			float scale = (float)Math.Ceiling(Math.Pow((float)2, (float)mipmap));
			int mipW = (int)(Width / scale);
			int mipH = (int)(Height / scale);

			if (compression == TXDCompression.DXT1 || compression == TXDCompression.DXT3 || compression == TXDCompression.DXT5)
			{
				if (mipW < 4)
					mipW = 4;
				if (mipH < 4)
					mipH = 4;
			}

			int mipSize = mipW * mipH;

			switch (compression)
			{
				case TXDCompression.NONE:
					mipSize *= bpp;
					break;
				case TXDCompression.DXT1:
					mipSize /= 2;
					break;
				case TXDCompression.DXT3:
				case TXDCompression.DXT5:
					// Size already correct
					break;
			}

			return mipSize;
		}

		public int ComputeDataSize(TXDCompression compression)
		{
			int size = 0;

			for (int i = 0; i < MipmapCount; i++)
			{
				size += ComputeMipmapDataSize(compression, i);
			}

			if ((RasterFormat & (int)TXDRasterFormat.RasterFormatEXTPAL4) != 0)
			{
				size += 16 * 4;
			}
			if ((RasterFormat & (int)TXDRasterFormat.RasterFormatEXTPAL8) != 0)
			{
				size += 256 * 4;
			}

			return size;
		}

		public RWTextureNativeData Clone()
        {
			return new RWTextureNativeData()
			{
				Platform = this.Platform,
				FilterFlags = this.FilterFlags,
				VWrap = this.VWrap,
				UWrap = this.UWrap,
				DiffuseName = this.DiffuseName,
				AlphaName = this.AlphaName,
				RasterFormat = this.RasterFormat,
				AlphaOrCompression = this.AlphaOrCompression,
				Width = this.Width,
				Height = this.Height,
				Bpp = this.Bpp,
				MipmapCount = this.MipmapCount,
				RasterType = this.RasterType,
				CompressionOrAlpha = this.CompressionOrAlpha,
				RawData = this.RawData,
			};
        }
	}
}
