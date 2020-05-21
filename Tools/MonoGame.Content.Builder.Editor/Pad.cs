// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Forms;

namespace MonoGame.Tools.Pipeline
{
    public partial class Pad
    {
        private ContextMenu _contextMenu;

        public Pad()
        {
            InitializeComponent();

            _contextMenu = new ContextMenu();

            _imageSettings.MouseDown += ImageSettings_MouseDown;
        }

        public string Title
        {
            get { return _labelTitle.Text; }
            set { _labelTitle.Text = value; }
        }

        public void SetMainContent(Control control)
        {
            _layoutMain.AddRow(control);
        }

        private void ImageSettings_MouseDown(object sender, MouseEventArgs e)
        {
            _contextMenu.Show(_imageSettings);
        }
    }
}
