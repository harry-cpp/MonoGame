// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Tools.Pipeline
{
    public partial class Pad : Panel
    {
        private DynamicLayout _layoutMain;
        private ImageView _imageSettings;
        private Label _labelTitle;

        private void InitializeComponent()
        {
            _layoutMain = new DynamicLayout();

            var panelLabel = new Panel();
            panelLabel.Padding = new Padding(5);
            panelLabel.Height = 25;

            var stack = new StackLayout();
            stack.Orientation = Orientation.Horizontal;

            _labelTitle = new Label();
            // _labelTitle.Font = new Font(_labelTitle.Font.Family, _labelTitle.Font.Size - 1, FontStyle.Bold);
            stack.Items.Add(new StackLayoutItem(_labelTitle, true));

            _imageSettings = new ImageView();
            _imageSettings.Image = Global.GetEtoIcon("Icons.Settings.png");
            _imageSettings.Visible = false;
            stack.Items.Add(new StackLayoutItem(_imageSettings, false)); 

            panelLabel.Content = stack;

            _layoutMain.AddRow(panelLabel);

            Content = _layoutMain;
        }
    }
}
