namespace RenderWareLib.Utils
{
    /// <summary>
    /// Provides Utilities to work with Arrays.
    /// </summary>
    public static class ArrayUtils
    {
        /// <summary>
        /// Join an string array in an unique string.
        /// </summary>
        /// <param name="param">Input strings.</param>
        /// <returns>Returns the joined array.</returns>
        public static string Join(string[] param, string separator = "")
        {
            string str = "";
            for (int i = 0; i < param.Length; i += 1)
            {
                if (str == "")
                {
                    str = param[i];
                }
                else
                {
                    str += separator + param[i];
                }
            }
            return str;
        }
    }
}
