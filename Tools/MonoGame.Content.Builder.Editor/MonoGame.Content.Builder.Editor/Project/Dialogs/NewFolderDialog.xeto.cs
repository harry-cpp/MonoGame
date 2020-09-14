// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace MonoGame.Content.Builder.Editor.Project
{
    public class NewFolderDialog : Dialog<DialogResult>
    {
        private TextBox _textBoxName = null;
        private Button _buttonCreate = null;

        public NewFolderDialog()
        {
            XamlReader.Load(this, "MonoGame.Content.Builder.Editor.Project.Dialogs.NewFolderDialog.xeto");
            Result = DialogResult.Cancel;
        }

        public string FolderName => _textBoxName.Text;

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
