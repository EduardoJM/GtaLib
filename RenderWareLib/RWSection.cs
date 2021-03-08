using System;
using System.IO;
using System.Collections.Generic;

namespace RenderWareLib
{
    /// <summary>
    /// Provides a class to work with RenderWare Binary Stream File Sections.
    /// </summary>
    public class RWSection
    {
        #region Static Methods
        /// <summary>
        /// Read a RWSection body content.
        /// </summary>
        /// <param name="br">The BinaryReader.</param>
        /// <param name="header">The Section Header.</param>
        /// <returns>The complete readed Section.</returns>
        public static RWSection ReadSectionBody(BinaryReader br, RWSectionHeader header)
        {
            RWSection sect = new RWSection(header);
            bool dataSect = !RWSectionInfo.RWIsSectionContainer(header.Id);
            System.Diagnostics.Debug.Print(sect.GetDescription() + " - " + dataSect.ToString());
            if (dataSect)
            {
                if (br.BaseStream.Position + header.Size > br.BaseStream.Length)
                {
                    throw new RWBinaryStreamException(
                        "Premature end of RWBS file: Data section ",
                        sect.GetDescription(),
                        "."
                    );
                }
                else
                {
                    sect.Data = br.ReadBytes((int)header.Size);
                }
            }
            else
            {
                sect.Data = null;
                RWSection newLastRead = sect;
                uint numRead = 0;
                while (numRead < sect.Header.Size)
                {
                    RWSection child = RWSection.ReadSection(br, newLastRead);
                    System.Diagnostics.Debug.Print((child == null).ToString());
                    child.Parent = sect;
                    numRead += child.Header.Size + 12;
                    sect.Children.Add(child);
                    newLastRead = child;
                }
            }
            return sect;
        }

        /// <summary>
        /// Read a RWSection.
        /// </summary>
        /// <param name="br">The BinaryReader.</param>
        /// <param name="lastRead">The last readed Section.</param>
        /// <returns>A readed section or null if not possible to read.</returns>
        public static RWSection ReadSection(BinaryReader br, RWSection lastRead)
        {
            RWSectionHeader header;
            if (!RWSectionHeader.ReadSectionHeader(br, out header, lastRead != null ? lastRead.Header : null))
            {
                return null;
            }
            return ReadSectionBody(br, header);
        }
        #endregion

        #region Fields
        private byte[] _data = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Section Header.
        /// </summary>
        public RWSectionHeader Header { get; private set; }

        /// <summary>
        /// Gets the Section Parent.
        /// </summary>
        public RWSection Parent { get; private set; }

        /// <summary>
        /// Gets the Section Children Collection.
        /// </summary>
        public List<RWSection> Children { get; private set; } = new List<RWSection>();

        /// <summary>
        /// Gets or Sets the Section Data;
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            set
            {
                if (value == null || value.Length == 0)
                {
                    _data = value;
                    if (Header != null)
                    {
                        Header.Size = 0;
                    }
                }
                else
                {
                    _data = value;
                    Header.Size = (uint)_data.Length;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of RWSection.
        /// </summary>
        /// <param name="_id">The Section Identifier.</param>
        /// <param name="_version">The Section RenderWare Version.</param>
        public RWSection(RWSectionId _id, uint _version)
        {
            Header = new RWSectionHeader(_id, 0, _version);
            Parent = null;
            Data = null;
        }

        /// <summary>
        /// Creates a new instance of RWSection.
        /// </summary>
        /// <param name="_id">The Section Identifier.</param>
        /// <param name="parent">The parent Section.</param>
        public RWSection(RWSectionId _id, RWSection parent)
        {
            Header = new RWSectionHeader(_id, 0, parent.Header.Version);
            Data = null;
            Parent = parent;
            Parent.AddChild(this);
        }

        /// <summary>
        /// Creates a new instance of RWSection.
        /// </summary>
        /// <param name="header">The Section Header.</param>
        public RWSection(RWSectionHeader header)
        {
            Header = header;
            Data = null;
            Parent = null;
        }

        /// <summary>
        /// Creates a new instance of RWSection.
        /// </summary>
        /// <param name="_rawData">The Section Raw Data.</param>
        public RWSection(byte[] _rawData)
        {
            Parent = null;
            Data = null;
            if (_rawData.Length < 12)
            {
                throw new RWBinaryStreamException(
                    "Invalid data size for RWSection(uint8_t*, size_t): Must be at least 12 bytes, but was ",
                    _rawData.Length.ToString(),
                    " bytes."
                );
            }

            Header = new RWSectionHeader(
                (RWSectionId)BitConverter.ToUInt32(_rawData, 0),
                BitConverter.ToUInt32(_rawData, 4),
                BitConverter.ToUInt32(_rawData, 8)
            );
            bool isDataSect = !RWSectionHeader.IsContainerSection(Header.Id);
            int readedSize = 12;
            if (_rawData.Length - readedSize < Header.Size)
            {
                throw new RWBinaryStreamException(
                    "Premature end of RWBS data: Section ",
                    GetDescription(),
                    " ended after ",
                    (_rawData.Length - readedSize).ToString(),
                    " bytes."
                );
            }

            if (isDataSect)
            {
                Data = new byte[Header.Size];
                Array.Copy(_rawData, readedSize, Data, 0, Header.Size);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(_rawData))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        br.BaseStream.Position += readedSize;
                        while (br.BaseStream.Position < br.BaseStream.Length)
                        {
                            if (br.BaseStream.Length - br.BaseStream.Position < 12)
                            {
                                ClearChildren();
                                throw new RWBinaryStreamException(
                                    "Section ",
                                    GetDescription(),
                                    " is not a valid container section: ",
                                    (br.BaseStream.Length - br.BaseStream.Position).ToString(),
                                    " dangling bytes at section end."
                                );
                            }
                            try
                            {
                                RWSection child = new RWSection(br.ReadBytes((int)Header.Size));
                                child.Parent = this;
                                Children.Add(child);
                            }
                            catch (Exception e)
                            {
                                ClearChildren();
                                throw;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Get an string description of the Section.
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            return RWSectionInfo.RWGetSectionName(Header.Id) + " (" + Header.Size + " bytes)";
        }

        /// <summary>
        /// Clear all the Section Childrens.
        /// </summary>
        public void ClearChildren()
        {
            if (Data == null)
            {
                for (int i = 0; i < Children.Count; i += 1)
                {
                    if (Children[i].Parent == this)
                    {
                        Children[i].Parent = null;
                    }
                }
                Children.Clear();
                if (Parent != null)
                {
                    // TODO: childResized
                    Parent.RecalculateSize();
                }
            }
        }

        /// <summary>
        /// Add a Child Section to this Section.
        /// </summary>
        /// <param name="_child">The Child Section to add.</param>
        public void AddChild(RWSection _child)
        {
            EnsureContainer();

            Header.Size += _child.Header.Size + 12;
            _child.Parent = this;
            Children.Add(_child);
            if (Parent != null)
            {
                // TODO: childResized
                Parent.RecalculateSize();
            }
        }

        public void EnsureContainer()
        {
            if (Data == null)
            {
                return;
            }
            if (Header.IsContainer())
            {
                ClearChildren();
                using (MemoryStream ms = new MemoryStream(Data))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        while (br.BaseStream.Position < br.BaseStream.Length)
                        {
                            if (br.BaseStream.Length - br.BaseStream.Position < 12)
                            {
                                ClearChildren();
                                throw new RWBinaryStreamException(
                                    "Section ",
                                    GetDescription(),
                                    " is not a valid container section: ",
                                    (br.BaseStream.Length - br.BaseStream.Position).ToString(),
                                    " dangling bytes at section end."
                                );
                            }
                            try
                            {
                                RWSection child = new RWSection(br.ReadBytes((int)Header.Size));
                                child.Parent = this;
                                Children.Add(child);
                            }
                            catch
                            {
                                ClearChildren();
                                throw;
                            }
                        }
                    }
                }
                Data = null;
            }
        }

        public RWSection[] FindChildCollection(RWSectionId id)
        {
            EnsureContainer();
            List<RWSection> sects = new List<RWSection>();
            for (int i = 0; i < Children.Count; i += 1)
            {
                if (Children[i].Header.Id == id)
                {
                    sects.Add(Children[i]);
                }
            }
            return sects.ToArray();
        }
        public RWSection FindChild(RWSectionId id)
        {
            EnsureContainer();
            for (int i = 0; i < Children.Count; i += 1)
            {
                if (Children[i].Header.Id == id)
                {
                    return Children[i];
                }
            }
            return null;
        }

        public uint RecalculateSize()
        {
            return 0;
            if (!Header.IsContainer())
            {
                Header.Size = (uint)Data.Length;
            } else
            {
                EnsureContainer();
                uint size = 0;
                for (int i = 0; i < Children.Count; i += 1)
                {
                    size += Children[i].RecalculateSize();
                }
                Header.Size = size;
            }
            return Header.Size;
        }

        public void Write(BinaryWriter bw)
        {
            Header.Write(bw);
            if (!Header.IsContainer())
            {
                bw.Write(Data);
            }
            else
            {
                for (int i = 0; i < Children.Count; i += 1)
                {
                    Children[i].Write(bw);
                }
            }
        }
    }
}
