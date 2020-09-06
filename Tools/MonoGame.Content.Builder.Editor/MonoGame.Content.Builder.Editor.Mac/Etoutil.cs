using AppKit;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Mac
{
    public static class Etoutil
    {
        public static NSView ToNative(this Control control)
        {
            return control.ToNative(true);
        }
    }
}
