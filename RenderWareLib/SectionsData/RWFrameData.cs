using System.Text;

namespace RenderWareLib.SectionsData
{
    public class RWFrameData : RWSectionData
    {
        public string FrameName { get; set; }

        public override void Parse(RWSection section)
        {
            FrameName = Encoding.ASCII.GetString(section.Data).Trim();
        }
    }
}
