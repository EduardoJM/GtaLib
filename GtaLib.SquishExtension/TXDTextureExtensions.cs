using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using GtaLib.TXD;

using RenderWareLib.SectionsData.TXD;

namespace GtaLib.Squish
{
    /// <summary>
    /// Provides Squish Optional Methods for TXDTexture.
    /// </summary>
    public static class TXDTextureExtensions
    {
        /// <summary>
        /// Get Bitmap Image From TXDTexture.
        /// </summary>
        /// <param name="texture">The TXDTexture.</param>
        /// <param name="level">The MipMap Level.</param>
        /// <returns>Returns the <see cref="System.Drawing.Bitmap"/> corresponding of the TXDTexture data.</returns>
        public static Bitmap GetBitmapImage(this TXDTexture texture, int level)
        {
            TXDTextureMipMapData[] levels = texture.GetMipLevelsData();
            if (level < 0 || level >= levels.Length)
            {
                level = 0;
            }
            TXDCompression compr = texture.Compression;
            if (compr == TXDCompression.DXT1 || compr == TXDCompression.DXT3 || compr == TXDCompression.DXT5)
            {
                Bitmap tmp = new Bitmap(levels[level].Width, levels[level].Height, PixelFormat.Format32bppArgb);
                byte[] rawData = levels[level].RasterData;
                SquishFlags flag = SquishFlags.Dxt1;
                if (compr == TXDCompression.DXT3)
                {
                    flag = SquishFlags.Dxt3;
                }
                else if (compr == TXDCompression.DXT5)
                {
                    flag = SquishFlags.Dxt5;
                }
                byte[] bgra = Squish.DecompressImage(rawData, tmp.Width, tmp.Height, flag);
                BitmapData tmpData = tmp.LockBits(new Rectangle(Point.Empty, tmp.Size), ImageLockMode.WriteOnly, tmp.PixelFormat);
                Marshal.Copy(bgra, 0, tmpData.Scan0, bgra.Length);
                tmp.UnlockBits(tmpData);
                return tmp;
            }
            else
            {
                throw new NotImplementedException("Raster Format " + texture.RasterFormat.ToString() + " not supported yet.");
            }
        }
    }
}
