using System;

using OpenTK.Graphics.OpenGL;

namespace GtaLib.Renderer.Utils
{
    internal class SafeEnableTexture2D : IDisposable
    {
        public bool IsEnabled { get; private set; }

        public SafeEnableTexture2D()
        {
            IsEnabled = GL.IsEnabled(EnableCap.Texture2D);
            if (!IsEnabled)
            {
                GL.Enable(EnableCap.Texture2D);
            }
        }

        public void Dispose()
        {
            if (!IsEnabled)
            {
                GL.Disable(EnableCap.Texture2D);
            }
        }
    }
}
