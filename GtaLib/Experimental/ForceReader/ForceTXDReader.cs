using System.IO;
using System.Text;

using RenderWareLib;

namespace GtaLib.Experimental.ForceReader
{
    public class ForceTXDReader
    {
        private BinaryReader br;

        private RWSectionHeader Header { get; set; }
        private RWSectionHeader PreviousHeader { get; set; }

        private RWSection TextureDictionary { get; set; }

        public ForceTXDReader(BinaryReader br)
        {
            this.br = br;
        }

        public RWSection Read()
        {
            ReadHeader();
            RWSection ext = new RWSection(RWSectionId.RW_SECTION_EXTENSION, TextureDictionary);
            // TextureDictionary.Children.Add(ext);
            TextureDictionary.RecalculateSize();
            return TextureDictionary;
        }

        private void ReadHeader()
        {
            PreviousHeader = Header;
            if (br.BaseStream.Position + 12 < br.BaseStream.Length)
            {
                Header = new RWSectionHeader((RWSectionId)br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32());
                System.Diagnostics.Debug.Print("Reading Header: " + Header.Id.ToString());
                ParseSection();
            }
        }

        private void ParseSection()
        {
            switch (Header.Id)
            {
                case RWSectionId.RW_SECTION_STRUCT:
                    ParseData();
                    break;
                case RWSectionId.RW_SECTION_EXTENSION:
                    ParseExtension();
                    break;
                case RWSectionId.RW_SECTION_TEXTURENATIVE:
                    ParseTexture();
                    break;
                case RWSectionId.RW_SECTION_TEXTUREDICTIONARY:
                    ParseTXDArchive();
                    break;
            }
        }

        private void ParseData()
        {
            switch (PreviousHeader.Id)
            {
                case RWSectionId.RW_SECTION_TEXTURENATIVE:
                    ParseTextueData();
                    break;
                case RWSectionId.RW_SECTION_TEXTUREDICTIONARY:
                    ParseTXDArchiveData();
                    break;
            }
        }

        private void ParseExtension()
        {
            System.Diagnostics.Debug.Print("Parsing Extension With " + Header.Size + " bytes");
            br.BaseStream.Position += Header.Size;
            ReadHeader();
        }

        private void ParseTexture()
        {
            System.Diagnostics.Debug.Print("Parsing Texture");
            ReadHeader();
        }

        private void ParseTXDArchive()
        {
            System.Diagnostics.Debug.Print("Parsing TXD Archive");
            uint gameType = Header.Version;
            TextureDictionary = new RWSection(RWSectionId.RW_SECTION_TEXTUREDICTIONARY, gameType);
            System.Diagnostics.Debug.Print("Version: " + gameType);
            // if (gameType == 134283263 || gameType == 268697599 || gameType == 402915327 || gameType == 201523199 || gameType == 67239935)
            {
                ReadHeader();
            }
        }

        private void ParseTXDArchiveData()
        {
            System.Diagnostics.Debug.Print("Parsing TXD Archive Data");
            // short textureCount = br.ReadInt16();
            // short unknown = br.ReadInt16();

            RWSection TextureDictionaryStruct = new RWSection(RWSectionId.RW_SECTION_STRUCT, TextureDictionary);
            TextureDictionaryStruct.Data = br.ReadBytes(4);
            //TextureDictionary.Children.Add(TextureDictionaryStruct);

            // textures = new TXDTexture[textureCount];
            // texturePosition = 0;
            ReadHeader();
        }

        private void ParseTextueData()
        {
            System.Diagnostics.Debug.Print("Parsing Texture Data");
            // br.BaseStream.Position += Header.Size;
            br.BaseStream.Position -= 12L;
            RWSection sec = new RWSection(br.ReadBytes((int)Header.Size + 12));
            RWSection textureNative = new RWSection(RWSectionId.RW_SECTION_TEXTURENATIVE, TextureDictionary);
            textureNative.Children.Add(sec);
            RWSection ext = new RWSection(RWSectionId.RW_SECTION_EXTENSION, textureNative);
            // textureNative.Children.Add(ext);
            // TextureDictionary.Children.Add(textureNative);
            ReadHeader();
        }
    }
}
