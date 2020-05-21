// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.ProjectView
{
    public partial class FileProgressDialog : Dialog
    {
        private Label _labelText;
        
        private void InitializeComponent()
        {
            Title = "File Operation in Progress";
            Size = new Size(400, -1);
            Padding = 8;
            Resizable = false;

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);

            _labelText = new Label();
            _labelText.Text = "Initializing...";
            layout.Add(_labelText, true, false);

            var progressBar = new ProgressBar();
            progressBar.Indeterminate = true;
            layout.Add(progressBar, true, false);

            Content = layout;
        }
    }
}
