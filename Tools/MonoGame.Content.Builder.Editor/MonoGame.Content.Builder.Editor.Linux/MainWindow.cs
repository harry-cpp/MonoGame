// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.IO;
using Gtk;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using Command = Eto.Forms.Command;

namespace MonoGame.Content.Builder.Editor.Linux
{
    partial class MainWindow : Window, IView
    {
        [Gtk.Builder.Object("headerbar")]
        private HeaderBar _headerBar = null!;

        [Gtk.Builder.Object("build_buttonbox")]
        private Widget _buildBox = null!;

        [Gtk.Builder.Object("cancel_button")]
        private Widget _cancelBox = null!;

        [Gtk.Builder.Object("separator1")]
        private Widget _boxSeparator = null!;

        [Gtk.Builder.Object("popovermenu1")]
        private PopoverMenu _popovermenu1 = null!;

        [Gtk.Builder.Object("popovermenu2")]
        private PopoverMenu _popovermenu2 = null!;

        private IconTheme _theme;

        public MainWindow() : base(WindowType.Toplevel)
        {
            var builder = new Gtk.Builder("MainWindow.glade");
            builder.Autoconnect(this);

            DefaultSize = new Gdk.Size(800, 600);
            Titlebar = _headerBar;
            Margin = 4;

            _theme = IconTheme.Default;

            Destroyed += (o, e) => Gtk.Application.Quit();
        }

        public void Attach(IController controller, Pad projectPad, Pad propertyPad)
        {
            var hpanned = new HPaned();
            var vpanned = new VPaned();

            hpanned.Pack1(vpanned, false, false);
            hpanned.Pack2(new TextView(), true, false);
            hpanned.Position = 200;

            vpanned.Pack1(projectPad.Control.ToNative(), true, false);
            vpanned.Pack2(propertyPad.Control.ToNative(), true, false);

            ConnectAction("new", controller.Commands.NewProject);
            ConnectAction("open", controller.Commands.OpenProject);
            ConnectAction("import", controller.Commands.ImportProject);
            ConnectAction("save", controller.Commands.SaveProject);
            ConnectAction("saveas", controller.Commands.SaveAsProject);
            ConnectAction("close", controller.Commands.CloseProject);

            Child = hpanned;
            Child.ShowAll();
        }

        public void UpdateEnabledCommands()
        {
            _buildBox.Visible = false;
            _cancelBox.Visible = false;
            _boxSeparator.Visible = _buildBox.Visible || _cancelBox.Visible;
        }

        public Eto.Drawing.Image GetFileIcon(string? filePath)
        {
            Gdk.Pixbuf? icon = null;

            if (File.Exists(filePath))
            {
                var file = GLib.FileFactory.NewForPath(filePath);
                var info = file.QueryInfo("standard::*", GLib.FileQueryInfoFlags.None, null);
                var sicon = info.Icon.ToString().Split(' ');

                for (int i = sicon.Length - 1; i >= 1; i--)
                {
                    try
                    {
                        icon = _theme.LoadIcon(sicon[i], 16, 0);
                        if (icon != null)
                            break;
                    }
                    catch { }
                }
            }

            return new Bitmap(new BitmapHandler(icon ?? _theme.LoadIcon("text-x-generic", 16, 0)));
        }

        public Eto.Drawing.Image GetFolderIcon()
        {
            return new Bitmap(new BitmapHandler(_theme.LoadIcon("folder", 16, 0)));
        }

        public Eto.Drawing.Image GetLinkIcon()
        {
            var linkIcon = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, 16, 16);
            linkIcon.Fill(0x00000000);
            _theme.LoadIcon("emblem-symbolic-link", 16, 0).Composite(linkIcon, 8, 8, 8, 8, 8, 8, 0.5, 0.5, Gdk.InterpType.Tiles, 255);
            return new Bitmap(new BitmapHandler(linkIcon));
        }

        private void ConnectAction(string action, Command command)
        {
            var simpleAction = new GLib.SimpleAction(action, null);
            simpleAction.Activated += (o, args) => 
            {
                _popovermenu1.Hide();
                _popovermenu2.Hide();
                command.Execute();
            };

            command.EnabledChanged += (sender, e) => simpleAction.Enabled = command.Enabled;
            Program.App.AddAction(simpleAction);
        }
    }
}

