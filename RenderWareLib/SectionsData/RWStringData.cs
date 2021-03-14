using System.Text;

namespace RenderWareLib.SectionsData
{
    public class RWStringData : RWSectionData
    {
        public string Text { get; set; }

        public override void Parse(RWSection section)
        {
            Text = Encoding.ASCII.GetString(section.Data).Trim();
        }
    }
}
