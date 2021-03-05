namespace RenderWareLib.Utils
{
    public static class ArrayUtils
    {
        public static string Join(string[] param)
        {
            string str = "";
            for (int i = 0; i < param.Length; i += 1)
            {
                str += param[i];
            }
            return str;
        }
    }
}
