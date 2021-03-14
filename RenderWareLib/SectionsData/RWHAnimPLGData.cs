using System;
using System.Collections.Generic;

namespace RenderWareLib.SectionsData
{
    public class RWHAnimPLGDataBone
    {
        public uint BoneId { get; set; }

        public uint Index { get; set; }

        public uint Flags { get; set; }
    }

    public class RWHAnimPLGData : RWSectionData
    {
        public uint Version { get; set; }

        public uint BoneID { get; set; }

        public uint BoneCount { get; set; }

        public uint Flags { get; set; }

        public uint KeyFrameSize { get; set; }

        public List<RWHAnimPLGDataBone> BoneInformations { get; private set; } = new List<RWHAnimPLGDataBone>();

        public override void Parse(RWSection section)
        {
            byte[] rawData = section.Data;
            Version = BitConverter.ToUInt32(rawData, 0);
            BoneID = BitConverter.ToUInt32(rawData, 4);
            BoneCount = BitConverter.ToUInt32(rawData, 8);
            int pos = 12;
            if (BoneCount > 0)
            {
                Flags = BitConverter.ToUInt32(rawData, pos);
                KeyFrameSize = BitConverter.ToUInt32(rawData, pos + 4);
                pos += 8;
                for (int i = 0; i < BoneCount; i += 1)
                {
                    RWHAnimPLGDataBone bone = new RWHAnimPLGDataBone()
                    {
                        Index = BitConverter.ToUInt32(rawData, pos),
                        BoneId = BitConverter.ToUInt32(rawData, pos + 4),
                        Flags = BitConverter.ToUInt32(rawData, pos + 8)
                    };
                    BoneInformations.Add(bone);
                    pos += 12;
                }
            }
        }
    }
}
