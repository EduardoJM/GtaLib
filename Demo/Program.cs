using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Demo
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            /*
                var nativeWindowSettings = new NativeWindowSettings()
                {
                    Size = new Vector2i(800, 600),
                    Title = "LearnOpenTK - Textures",
                };

                using (var window = new GameWindowDemo(GameWindowSettings.Default, nativeWindowSettings))
                {
                    window.Run();
                }
            */
        }
    }
}
