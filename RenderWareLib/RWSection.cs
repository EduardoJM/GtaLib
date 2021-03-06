﻿using System;
using System.IO;
using System.Collections.Generic;

namespace RenderWareLib
{
    public class RWSection
    {
        private RWSectionHeader _header;

        private ulong _offset;

        private RWSection _parent;

        private byte[] _data;

        public List<RWSection> Children { get; private set; } = new List<RWSection>();

        public byte[] Data { get { return _data; } set { _data = value; } }

        public RWSection(RWSectionId _id, uint _version)
        {
            _header = new RWSectionHeader(_id, 0, _version);
            _offset = 0L;
            _parent = null;
            _data = null;
        }

        public RWSection(RWSectionId _id, RWSection parent)
        {
            _header = new RWSectionHeader(_id, 0, parent._header.Version);
            _offset = 0L;
            _data = null;
            parent.AddChild(this);
            _parent = parent;
        }

        public RWSection(RWSectionHeader header)
        {
            _header = header;
            _offset = 0;
            _data = null;
            _parent = null;
        }

        public static RWSection ReadSectionBody(BinaryReader br, RWSectionHeader header)
        {
            RWSection sect = new RWSection(header);
            sect._offset = 0;
            bool dataSect = !RWSectionInfo.RWIsSectionContainer(header.Id);
            System.Diagnostics.Debug.Print(sect.GetDescription() + " - " + dataSect.ToString());
            if (dataSect)
            {
                /*
                if (header.Id == RWSectionId.RW_SECTION_STRUCT && lastRead != null)
                {
                    System.Diagnostics.Debug.Print("Section " +header.Id.ToString() +  " Data Size: " + header.Size);
                    if (lastRead._header.Id == RWSectionId.RW_SECTION_CLUMP)
                    {
                        header.Size = 12;
                    } else if (lastRead._header.Id == RWSectionId.RW_SECTION_MATERIALLIST)
                    {
                        header.Size = 4;
                    }
                }
                */
                if (br.BaseStream.Position + header.Size > br.BaseStream.Length)
                {
                    /*
                    throw new RWBinaryStreamException(
                        "Premature end of RWBS file: Data section ",
                        sect.GetDescription(),
                        "."
                    );
                    */
                    // TODO: create a method to change skip method
                }
                else
                {
                    sect._data = br.ReadBytes((int)header.Size);
                    if (sect._header.Id == RWSectionId.RW_SECTION_STRING)
                    {
                        System.Diagnostics.Debug.Print(" @ String: " + System.Text.Encoding.ASCII.GetString(sect._data));
                    }
                }
            }
            else
            {
                sect._data = null;
                RWSection newLastRead = sect;
                uint numRead = 0;
                while (numRead < sect._header.Size)
                {
                    RWSection child = RWSection.ReadSection(br, newLastRead);
                    System.Diagnostics.Debug.Print((child == null).ToString());
                    child._parent = sect;
                    numRead += child._header.Size + 12;
                    sect.Children.Add(child);
                    newLastRead = child;
                }
            }
            return sect;
        }

        public static RWSection ReadSection(BinaryReader br, RWSection lastRead)
        {
            RWSectionHeader header;
            if (!RWSectionHeader.ReadSectionHeader(br, out header, lastRead != null ? lastRead._header : null))
            {
                System.Diagnostics.Debug.Print("Failed on header");
                return null;
            }
            return ReadSectionBody(br, header);
        }

        public string GetDescription()
        {
            return RWSectionInfo.RWGetSectionName(_header.Id) + " (" + _header.Size + " bytes @ 0x" + _offset.ToString("X") + ")";
        }

        public RWSection(byte[] _rawData)
        {
            _offset = 0L;
            _parent = null;
            _data = null;
            if (_rawData.Length < 12)
            {
                throw new RWBinaryStreamException(
                    "Invalid data size for RWSection(uint8_t*, size_t): Must be at least 12 bytes, but was ",
                    _rawData.Length.ToString(),
                    " bytes."
                );
            }

            _header = new RWSectionHeader(
                (RWSectionId)BitConverter.ToUInt32(_rawData, 0),
                BitConverter.ToUInt32(_rawData, 4),
                BitConverter.ToUInt32(_rawData, 8)
            );
            bool isDataSect = !RWSectionHeader.IsContainerSection(_header.Id);
            int readedSize = 12;
            if (_rawData.Length - readedSize < _header.Size)
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
                _data = new byte[_header.Size];
                Array.Copy(_rawData, readedSize, _data, 0, _header.Size);
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(_data))
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
                                RWSection child = new RWSection(br.ReadBytes((int)_header.Size));
                                child._parent = this;
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


        public void ClearChildren()
        {
            if (_data == null)
            {
                for (int i = 0; i < Children.Count; i += 1)
                {
                    if (Children[i]._parent == this)
                    {
                        Children[i]._parent = null;
                    }
                }
                Children.Clear();
            }
        }

        public void AddChild(RWSection _child)
        {
            EnsureContainer();

            _header.Size += _child._header.Size + 12;
            _child._parent = this;
            Children.Add(_child);
            if (_parent != null)
            {
                // TODO:
                // _parent.ChildResized(sect->header.size + 12);
            }
        }

        public void EnsureContainer()
        {
            if (_data == null)
            {
                return;
            }
            if (_header.IsContainer())
            {
                ClearChildren();
                using (MemoryStream ms = new MemoryStream(_data))
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
                                RWSection child = new RWSection(br.ReadBytes((int)_header.Size));
                                child._parent = this;
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
                _data = null;
            }
        }

        public RWSection[] FindChildCollection(RWSectionId id)
        {
            EnsureContainer();
            List<RWSection> sects = new List<RWSection>();
            for (int i = 0; i < Children.Count; i += 1)
            {
                if (Children[i]._header.Id == id)
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
                if (Children[i]._header.Id == id)
                {
                    return Children[i];
                }
            }
            return null;
        }

        public uint RecalculateSize()
        {
            if (!_header.IsContainer())
            {
                _header.Size = (uint)_data.Length;
            } else
            {
                EnsureContainer();
                uint size = 0;
                for (int i = 0; i < Children.Count; i += 1)
                {
                    size += Children[i].RecalculateSize();
                }
                _header.Size = size;
            }
            return _header.Size;
        }

        public void Write(BinaryWriter bw)
        {
            _header.Write(bw);
            if (!_header.IsContainer())
            {
                bw.Write(_data);
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

