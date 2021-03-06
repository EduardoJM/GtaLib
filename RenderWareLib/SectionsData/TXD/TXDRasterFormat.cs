namespace RenderWareLib.SectionsData.TXD
{
    public enum TXDRasterFormat
    {
		RasterFormatDefault = 0x000,
		/// <summary>
		/// 1 bit alpha and 5 bit for each red, green and blue (DXT1/3).
		/// </summary>
		RasterFormatA1R5G5B5 = 0x100,
		/// <summary>
		/// 5 bits red, 6 bits green and 5 bits blue (DXT1/3).
		/// </summary>
		RasterFormatR5G6B5 = 0x200,
		/// <summary>
		/// 4 bits red, green, blue and alpha each (DXT1/3).
		/// </summary>
		RasterFormatR4G4B4A4 = 0x300,
		/// <summary>
		/// 8 bit luminance (for black-white).
		/// </summary>
		RasterFormatLUM8 = 0x400,
		/// <summary>
		/// 8 bits blue, green, red and alpha each (in this order).
		/// </summary>
		RasterFormatB8G8R8A8 = 0x500,
		/// <summary>
		/// 8 bits blue, green and red each (in this order).
		/// </summary>
		RasterFormatB8G8R8 = 0x600,
		/// <summary>
		/// 5 bits red, green and blue each.
		/// </summary>
		RasterFormatR5G5B5 = 0xA00,

		/// <summary>
		/// 8 bits red, green, blue and alpha each (in this order).
		/// </summary>
		RasterFormatR8G8B8A8 = 0xF00,

		/// <summary>
		/// Mipmaps shall be automatically generated.
		/// </summary>
		RasterFormatEXTAutoMipmap = 0x1000,
		/// <summary>
		/// 8 bit palette with 256 entries included.
		/// </summary>
		RasterFormatEXTPAL8 = 0x2000,
		/// <summary>
		/// 4 bit palette with 16 entries included.
		/// </summary>
		RasterFormatEXTPAL4 = 0x4000,
		/// <summary>
		/// Mipmaps included.
		/// </summary>
		RasterFormatEXTMipmap = 0x8000,

		/// <summary>
		/// Mask to strip out the base raster format.
		/// </summary>
		RasterFormatMask = 0xF00,
		/// <summary>
		/// Mask to strip out the raster format extension.
		/// </summary>
		RasterFormatEXTMask = 0xF000
	}
}
