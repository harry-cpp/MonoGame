// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Gtk;
using MonoGame.Content.Builder.Editor.Project;
using MonoGame.Content.Builder.Editor.Property;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;

namespace MonoGame.Content.Builder.Editor
{
    partial class MainWindow : Window, IView
    {
        [Gtk.Builder.Object("headerbar")]
        private HeaderBar _headerBar = null!;

        [Gtk.Builder.Object("build_buttonbox")]
        private Box _buildBox = null!;

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

        public void Attach(ProjectPad projectPad, PropertyPad propertyPad)
        {
            var hpanned = new HPaned();
            var vpanned = new VPaned();

            hpanned.Pack1(vpanned, true, true);
            hpanned.Pack2(new TextView(), true, true);
            hpanned.Position = 200;

            vpanned.Pack1(projectPad.ToNative(), true, true);
            vpanned.Pack2(propertyPad.ToNative(), true, true);

            Child = hpanned;

            ConnectAction("open", () => Controller.OpenProject());
        }

        private static void ConnectAction(string actionName, System.Action action)
        {
            var simpleAction = new GLib.SimpleAction(actionName, null);
            simpleAction.Activated += (o, args) => action();
            Program.App.AddAction(simpleAction);
        }

        public Eto.Drawing.Image GetFileIcon(string path, bool link)
        {
            Gdk.Pixbuf icon = null;

            var file = GLib.FileFactory.NewForPath(path);
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

        public Eto.Drawing.Image GetImageForResource(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateRecentList(List<string> recentList)
        {
            _buildBox.Visible = false;
        }
    }
}

