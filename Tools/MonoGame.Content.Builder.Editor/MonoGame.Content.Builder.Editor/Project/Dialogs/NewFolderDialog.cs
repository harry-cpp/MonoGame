// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Project
{
    public partial class NewFolderDialog : Dialog
    {
        private TextBox _textBoxName;
        private Button _buttonCreate, _buttonCancel;

        public NewFolderDialog()
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

            Content = dynamicLayout;

            _buttonCreate = new Button();
            _buttonCreate.Text = "Create Folder";
            PositiveButtons.Add(_buttonCreate);
            DefaultButton = _buttonCreate;

            _buttonCancel = new Button();
            _buttonCancel.Text = "Cancel";
            NegativeButtons.Add(_buttonCancel);
            AbortButton = _buttonCancel;

            Result = DialogResult.Cancel;

            _textBoxName.TextChanged += TextBoxName_TextChanged;
            _textBoxName.KeyUp += TextBoxName_KeyUp;
            _buttonCreate.Click += ButtonCreate_Click;
            _buttonCancel.Click += ButtonCancel_Click;
        }

        public string Text => _textBoxName.Text;

        public DialogResult Result { get; private set; }

        private void TextBoxName_TextChanged(object sender, EventArgs args)
        {
            _buttonCreate.Enabled = !string.IsNullOrEmpty(_textBoxName.Text) && Util.CheckString(_textBoxName.Text);
        }

        private void TextBoxName_KeyUp(object sender, KeyEventArgs args)
        {
            if (args.Key == Keys.Enter && _buttonCreate.Enabled)
            {
                Result = DialogResult.Ok;
                Close();
            }
        }

        private void ButtonCreate_Click(object sender, EventArgs args)
        {
            Result = DialogResult.Ok;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs args)
        {
            Close();
        }
    }
}
