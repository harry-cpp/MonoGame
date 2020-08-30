// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor
{
    public partial class Pad
    {
        private ContextMenu _contextMenu;
        private List<Command> _commands;
        private DynamicLayout _layoutMain;
        private ImageView _imageSettings;
        private Label _labelTitle;

        public Pad()
        {
            _layoutMain = new DynamicLayout();

            var panelLabel = new Panel();
            panelLabel.Padding = new Padding(5);
            panelLabel.Height = 25;

            var stack = new StackLayout();
            stack.Orientation = Orientation.Horizontal;

            _labelTitle = new Label();
            if (!(Util.IsGtk && Util.IsMac))
            {
                _labelTitle.Font = new Font(_labelTitle.Font.Family, _labelTitle.Font.Size - 1, FontStyle.Bold);
            }
            stack.Items.Add(new StackLayoutItem(_labelTitle, true));

            _imageSettings = new ImageView();
            _imageSettings.Image = Controller.GetImageForResource("Icons.Settings.png");
            _imageSettings.Visible = false;
            stack.Items.Add(new StackLayoutItem(_imageSettings, false)); 

            panelLabel.Content = stack;

            _layoutMain.AddRow(panelLabel);

            Content = _layoutMain;

            _contextMenu = new ContextMenu();
            _commands = new List<Command>();

            _imageSettings.MouseDown += ImageSettings_MouseDown;
        }

        public string Title
        {
            get { return _labelTitle.Text; }
            set { _labelTitle.Text = value; }
        }

        private void ImageSettings_MouseDown(object sender, MouseEventArgs e)
        {
            _contextMenu.Show(_imageSettings);
        }

        public void AddViewItem(Command command)
        {
            _imageSettings.Visible = true;

            _commands.Add(command);
            _contextMenu.Items.Add(command.CreateMenuItem());

            AddMenuItem("View/" + Title + "/" + command.MenuText, command);
        }

        public void AddMenuItem(string menuItemPath, Command command)
        {
            var mi = command.CreateMenuItem();
            mi.Text = Path.GetFileName(menuItemPath);

            // AddMenuItem(MainWindow.MainMenu.Items, mi, menuItemPath);
        }

        /*private void AddMenuItem(MenuItemCollection items, MenuItem item, string path)
        {
            var split = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (split.Length == 1)
            {
                items.Add(item);
                return;
            }
            
            ButtonMenuItem mi = null;

            foreach (var i in items)
            {
                if (i.Text == split[0])
                {
                    mi = i as ButtonMenuItem;
                    break;
                }
            }

            if (mi == null)
            {
                mi = new ButtonMenuItem();
                mi.Text = split[0];
                items.Add(mi);
            }

            AddMenuItem(mi.Items, item, string.Join('/', split, 1, split.Length - 1));
        }*/

        public void SetMainContent(Control control)
        {
            if (Util.IsIde)
                Content = control;
            else
                _layoutMain.AddRow(control);
        }
    }
}
