using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using GtaLib.TXD;

using RenderWareLib.SectionsData.TXD;

namespace GtaLib.Squish
{
    public static class TXDTextureExtensions
    {
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
                byte[] argb = bgra;// new byte[bgra.Length];
                /*
                for (int i = 0; i < bgra.Length; i += 4)
                {
                    byte b = bgra[i + 0];
                    byte g = bgra[i + 1];
                    byte r = bgra[i + 2];
                    byte a = bgra[i + 3];
                    argb[i + 0] = a;
                    argb[i + 1] = r;
                    argb[i + 2] = g;
                    argb[i + 3] = b;
                }
                */
                BitmapData tmpData = tmp.LockBits(new Rectangle(Point.Empty, tmp.Size), ImageLockMode.WriteOnly, tmp.PixelFormat);
                Marshal.Copy(argb, 0, tmpData.Scan0, argb.Length);
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
