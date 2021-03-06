using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using GtaLib.TXD;
using GtaLib.TXD.Utils;
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

                if (compr == TXDCompression.DXT1 || compr == TXDCompression.DXT3)
                {
                    TXDTextureMipMapData[] levels = texture.GetMipLevelsData();
                    for (int i = 0; i < levels.Length; i += 1)
                    {
                        GL.CompressedTexImage2D(
                            TextureTarget.Texture2D,
                            i,
                            compr == TXDCompression.DXT1 ? InternalFormat.CompressedRgbaS3tcDxt1Ext : InternalFormat.CompressedRgbaS3tcDxt3Ext,
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
                    // TODO: implements support for other formats (uncompressed) here
                    TXDConverter conv = new TXDConverter();
                    TXDTexture outTexture = texture.Clone();
                    outTexture.RasterFormat = TXDRasterFormat.RasterFormatR8G8B8A8;
                    List<TXDTextureMipMapData> outMipMaps = null;
                    if (conv.Convert(texture, outTexture, out outMipMaps))
                    {
                        for (int i = 0; i < outMipMaps.Count; i += 1)
                        {
                            GL.TexImage2D(
                                TextureTarget.Texture2D,
                                i,
                                PixelInternalFormat.Rgba,
                                outMipMaps[i].Width,
                                outMipMaps[i].Height,
                                0,
                                PixelFormat.Rgba,
                                PixelType.UnsignedByte,
                                outMipMaps[i].RasterData
                            );
                        }
                    }
                }
            }
            return id;
        }
    }
}
