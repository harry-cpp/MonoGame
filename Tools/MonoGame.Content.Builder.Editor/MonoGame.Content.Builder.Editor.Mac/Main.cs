using AppKit;

namespace MonoGame.Content.Builder.Editor.Mac
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();

            var etoApp = new Eto.Forms.Application(Eto.Platforms.XamMac2);
            etoApp.Attach();

            NSApplication.Main(args);
        }
    }
}

