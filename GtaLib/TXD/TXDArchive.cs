using System.IO;
using System.Collections.Generic;

using RenderWareLib;
using RenderWareLib.SectionsData;

namespace GtaLib.TXD
{
    public class TXDArchive
    {
        public List<TXDTexture> Textures { get; private set; } = new List<TXDTexture>();

        public static bool GotoTextureDictionarySection(BinaryReader br, out RWSectionHeader outHeader)
        {
            while (RWSectionHeader.ReadSectionHeader(br, out outHeader) && outHeader.Id != RWSectionId.RW_SECTION_TEXTUREDICTIONARY)
            {
                br.BaseStream.Position += outHeader.Size;
            }
            if (outHeader.Id == RWSectionId.RW_SECTION_TEXTUREDICTIONARY)
            {
                return true;
            }
            return false;
        }

        public void Read(BinaryReader br)
        {
            RWSectionHeader textureDictionaryHeader;
            if (GotoTextureDictionarySection(br, out textureDictionaryHeader))
            {
                // TODO: read sections here
                RWSection textureDictionary = RWSection.ReadSectionBody(br, textureDictionaryHeader);
                RWSection[] textureNativeCollection = textureDictionary.FindChildCollection(RWSectionId.RW_SECTION_TEXTURENATIVE);
                for(int i = 0; i < textureNativeCollection.Length;i+=1)
                {
                    RWSection data = textureNativeCollection[i].FindChild(RWSectionId.RW_SECTION_STRUCT);
                    RWTextureNativeData native = new RWTextureNativeData();
                    native.Parse(data);
                    Textures.Add(new TXDTexture(native));
                }
            }
        }
    }
}
