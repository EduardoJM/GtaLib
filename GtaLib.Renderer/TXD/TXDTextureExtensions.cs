using System;
using OpenTK.Graphics.OpenGL;

using GtaLib.TXD;
using GtaLib.Renderer.Utils;
using RenderWareLib.SectionsData.TXD;

namespace GtaLib.Renderer.TXD
{
    public static class TXDTextureExtensions
    {
        const int GL_REPEAT = 0x2901;
        const int GL_MIRRORED_REPEAT = 0x8370;
        const int GL_CLAMP_TO_EDGE = 0x812F;

        const int GL_NEAREST = 0x2600;
        const int GL_LINEAR = 0x2601;
        const int GL_NEAREST_MIPMAP_NEAREST = 0x2700;
        const int GL_LINEAR_MIPMAP_NEAREST = 0x2701;
        const int GL_NEAREST_MIPMAP_LINEAR = 0x2702;
        const int GL_LINEAR_MIPMAP_LINEAR = 0x2703;


        static byte[] FlipBGRAtoRGBA(byte[] data)
        {
            byte[] outData = new byte[data.Length];
            for (int i = 0; i < data.Length; i += 4)
            {
                // B8G8R8A8 -> RGBA
                byte b = data[i + 0];
                byte g = data[i + 1];
                byte r = data[i + 2];
                byte a = data[i + 3];
                outData[i + 0] = r;
                outData[i + 1] = g;
                outData[i + 2] = b;
                outData[i + 3] = a;
            }
            return outData;
        }

        public static int Upload(this TXDTexture texture)
        {
            int id = -1;
            using (SafeEnableTexture2D enableTexture2D = new SafeEnableTexture2D())
            {
                id = GL.GenTexture();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, id);
                int uWrap = GL_REPEAT;
                int vWrap = GL_REPEAT;
                switch (texture.UWrap)
                {
                    case TXDWrappingMode.WrapNone:
                        uWrap = GL_REPEAT;
                        break;
                    case TXDWrappingMode.WrapWrap:
                        uWrap = GL_REPEAT;
                        break;
                    case TXDWrappingMode.WrapMirror:
                        uWrap = GL_MIRRORED_REPEAT;
                        break;
                    case TXDWrappingMode.WrapClamp:
                        uWrap = GL_CLAMP_TO_EDGE;
                        break;
                }
                switch (texture.VWrap)
                {
                    case TXDWrappingMode.WrapNone:
                        vWrap = GL_REPEAT;
                        break;
                    case TXDWrappingMode.WrapWrap:
                        vWrap = GL_REPEAT;
                        break;
                    case TXDWrappingMode.WrapMirror:
                        vWrap = GL_MIRRORED_REPEAT;
                        break;
                    case TXDWrappingMode.WrapClamp:
                        vWrap = GL_CLAMP_TO_EDGE;
                        break;
                }
                int magFilter, minFilter;

                switch (texture.FilterFlags)
                {
                    case TXDFilterFlags.FilterLinear:
                        magFilter = GL_LINEAR;
                        minFilter = GL_LINEAR;
                        break;

                    case TXDFilterFlags.FilterNearest:
                        magFilter = GL_NEAREST;
                        minFilter = GL_NEAREST;
                        break;

                    case TXDFilterFlags.FilterMipNearest:
                        magFilter = GL_NEAREST;
                        minFilter = GL_NEAREST_MIPMAP_NEAREST;
                        break;

                    case TXDFilterFlags.FilterMipLinear:
                        magFilter = GL_NEAREST;
                        minFilter = GL_NEAREST_MIPMAP_LINEAR;
                        break;

                    case TXDFilterFlags.FilterLinearMipNearest:
                        magFilter = GL_LINEAR;
                        minFilter = GL_LINEAR_MIPMAP_NEAREST;
                        break;

                    case TXDFilterFlags.FilterLinearMipLinear:
                        magFilter = GL_LINEAR;
                        minFilter = GL_LINEAR_MIPMAP_LINEAR;
                        break;

                    case TXDFilterFlags.FilterNone:
                        // Don't know what it is
                        magFilter = GL_LINEAR;
                        minFilter = GL_LINEAR;
                        break;

                    default:
                        // Unknown filter, use linear
                        magFilter = GL_LINEAR;
                        minFilter = GL_LINEAR;
                        break;
                }
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, uWrap);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, vWrap);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, magFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, minFilter);

                TXDRasterFormat ext = texture.GetRasterFormatExtension();
                bool mipmapsIncluded = (ext & TXDRasterFormat.RasterFormatEXTMipmap) != 0;
                bool mipmapsAuto = (ext & TXDRasterFormat.RasterFormatEXTAutoMipmap) != 0;
                int numIncludedMipmaps = mipmapsIncluded ? texture.MipMapCount : 1;

                if (mipmapsAuto)
                {
                    // TODO: add some restrictions for this...
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
                }

                short w = texture.Width;
                short h = texture.Height;

                TXDCompression compr = texture.Compression;

                if (compr == TXDCompression.DXT1 || compr == TXDCompression.DXT3 || compr == TXDCompression.DXT5)
                {
                    TXDTextureMipMapData[] levels = texture.GetMipLevelsData();
                    for (int i = 0; i < levels.Length; i += 1)
                    {
                        InternalFormat internalF = InternalFormat.CompressedRgbaS3tcDxt1Ext;
                        if (compr == TXDCompression.DXT3)
                        {
                            internalF = InternalFormat.CompressedRgbaS3tcDxt3Ext;
                        }
                        else if (compr == TXDCompression.DXT5)
                        {
                            internalF = InternalFormat.CompressedRgbaS3tcDxt5Ext;
                        }
                        GL.CompressedTexImage2D(
                            TextureTarget.Texture2D,
                            i,
                            internalF,
                            levels[i].Width,
                            levels[i].Height,
                            0,
                            levels[i].RasterData.Length,
                            levels[i].RasterData
                        );
                        if (i == levels.Length - 1)
                        {
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, i - 1);
                        }
                    }
                }
                else
                {
                    if (texture.RasterFormat == TXDRasterFormat.RasterFormatB8G8R8A8)
                    {
                        TXDTextureMipMapData[] levels = texture.GetMipLevelsData();
                        for (int i = 0; i < levels.Length; i += 1)
                        {
                            GL.TexImage2D(
                                TextureTarget.Texture2D,
                                i,
                                PixelInternalFormat.Rgba,
                                levels[i].Width,
                                levels[i].Height,
                                0,
                                PixelFormat.Bgra,
                                PixelType.UnsignedByte,
                                levels[i].RasterData
                            );
                            if (i == levels.Length - 1)
                            {
                                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, i - 1);
                            }
                        }
                    }
                    else if (texture.RasterFormat == TXDRasterFormat.RasterFormatB8G8R8)
                    {
                        TXDTextureMipMapData[] levels = texture.GetMipLevelsData();
                        for (int i = 0; i < levels.Length; i += 1)
                        {
                            GL.TexImage2D(
                                TextureTarget.Texture2D,
                                i,
                                levels[i].RasterSize == levels[i].Width * levels[i].Height * 3 ? PixelInternalFormat.Rgb : PixelInternalFormat.Rgba,
                                levels[i].Width,
                                levels[i].Height,
                                0,
                                levels[i].RasterSize == levels[i].Width * levels[i].Height * 3 ? PixelFormat.Bgr : PixelFormat.Bgra,
                                PixelType.UnsignedByte,
                                levels[i].RasterData
                            );
                            if (i == levels.Length - 1)
                            {
                                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, i - 1);
                            }
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("Raster Format: " + texture.RasterFormat.ToString() + " not supported yet.");
                    }
                }
            }
            return id;
        }
    }
}
