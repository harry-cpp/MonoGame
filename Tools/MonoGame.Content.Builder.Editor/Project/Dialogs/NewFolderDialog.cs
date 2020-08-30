// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto.Forms;
using MonoGame.Tools.Pipeline;

namespace MonoGame.Content.Builder.Editor.Project
{
    public partial class NewFolderDialog : Dialog
    {
        public NewFolderDialog()
        {
            InitializeComponent();

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
            _buttonCreate.Enabled = !string.IsNullOrEmpty(_textBoxName.Text) && Global.CheckString(_textBoxName.Text);
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
