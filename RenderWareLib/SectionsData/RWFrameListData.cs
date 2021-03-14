using System;
using RenderWareLib.Mathematics;

namespace RenderWareLib.SectionsData
{
    public class RWFrameListDataItem
    {
        public RWMatrix4 TransformMatrix { get; set; }

        public uint ParentFrame { get; set; }

        public uint Flags { get; set; }
    }

    public class RWFrameListData : RWSectionData
    {
        public uint FramesCount { get; set; }

        public RWFrameListDataItem[] Items { get; set; }

        public override void Parse(RWSection section)
        {
            byte[] rawData = section.Data;
            FramesCount = BitConverter.ToUInt32(rawData, 0);
            Items = new RWFrameListDataItem[FramesCount];
            for (int i = 0; i < FramesCount; i += 1)
            {
                int pos = 4 + i * 56;
                Items[i] = new RWFrameListDataItem();
                Items[i].TransformMatrix = new RWMatrix4(
                        BitConverter.ToSingle(rawData, pos),
                        BitConverter.ToSingle(rawData, pos + 4),
                        BitConverter.ToSingle(rawData, pos + 8),
                        0.0f,

                        BitConverter.ToSingle(rawData, pos + 12),
                        BitConverter.ToSingle(rawData, pos + 16),
                        BitConverter.ToSingle(rawData, pos + 20),
                        0.0f,

                        BitConverter.ToSingle(rawData, pos + 24),
                        BitConverter.ToSingle(rawData, pos + 28),
                        BitConverter.ToSingle(rawData, pos + 32),
                        0.0f,

                        BitConverter.ToSingle(rawData, pos + 36),
                        BitConverter.ToSingle(rawData, pos + 40),
                        BitConverter.ToSingle(rawData, pos + 44),
                        1.0f
                );
                Items[i].ParentFrame = BitConverter.ToUInt32(rawData, pos + 12 * 4);
                Items[i].Flags = BitConverter.ToUInt32(rawData, pos + 12 * 4 + 4);
            }
        }
    }
}
