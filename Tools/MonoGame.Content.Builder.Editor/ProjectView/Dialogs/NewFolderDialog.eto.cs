// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.ProjectView
{
    public partial class NewFolderDialog : Dialog
    {
        private TextBox _textBoxName;
        private Label _labelError;
        private Button _buttonCreate, _buttonCancel;

        private void InitializeComponent()
        {
            DisplayMode =  DialogDisplayMode.Attached;
            Resizable = false;
            Width = 300;
            Title = "New Folder";
            Padding = 8;

            var dynamicLayout = new DynamicLayout();
            dynamicLayout.Spacing = new Eto.Drawing.Size(8, 8);

            dynamicLayout.Add(new Label { Text = "Folder Name:" }, true, false);

            _textBoxName = new TextBox();
            _textBoxName.Text = "Folder";
            _textBoxName.SelectAll();
            dynamicLayout.Add(_textBoxName, true, false);

            _labelError = new Label();
            dynamicLayout.Add(_labelError, true, false);

            Content = dynamicLayout;

            _buttonCreate = new Button();
            _buttonCreate.Text = "Create Folder";
            PositiveButtons.Add(_buttonCreate);
            DefaultButton = _buttonCreate;

            _buttonCancel = new Button();
            _buttonCancel.Text = "Cancel";
            NegativeButtons.Add(_buttonCancel);
            AbortButton = _buttonCancel;
        }
    }
}
