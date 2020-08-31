using Eto.Forms;

namespace MonoGame.Content.Builder.Editor
{
    public static class EtoUtil
    {
        public static Gtk.Widget ToNative(this Control control)
        {
            return control.ToNative(true);
        }
    }
}
