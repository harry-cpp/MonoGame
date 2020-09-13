// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto.GtkSharp.Forms;
using Application = Gtk.Application;

namespace MonoGame.Content.Builder.Editor.Linux
{
    public static class Program
    {
        public static Application App { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var etoApp = new Eto.Forms.Application(Eto.Platforms.Gtk);

            Eto.Style.Add<ApplicationHandler>("MGCBEditor", h =>
            {
                App = h.Control;
            });
            etoApp.Style = "MGCBEditor";

            // var app = App = new Application("net.monogame.mgcb-editor", ApplicationFlags.None);
            // app.Register(GLib.Cancellable.Current);

            etoApp.Attach();

            var window = new MainWindow();
            App.AddWindow(window);

            Util.IsMac = true;
            Util.IsGtk = true;
            Util.MainWindow = Eto.Forms.Gtk3Helpers.ToEtoWindow(window);
            var controller = new Controller(window);

            window.Show();
            Application.Run();
        }
    }
}
