// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace MonoGame.Content.Builder.Editor.Property
{
    public class PathDialog : Dialog<DialogResult>
    {
        private TextBox _textBoxPath = null;

        public PathDialog()
        {
            XamlReader.Load(this, "MonoGame.Content.Builder.Editor.Property.Dialogs.PathDialog.xeto");
        }

        public string FolderPath
        {
            get => _textBoxPath.Text;
            set => _textBoxPath.Text = value;
        }

        private void ButtonBrowse_Click(object sender, EventArgs args)
        {
            var openFolderDialog = new SelectFolderDialog();
            // TODO: openFolderDialog.Directory start location
            if (openFolderDialog.Show() == DialogResult.Ok)
                _textBoxPath.Text = openFolderDialog.Directory;
        }

        private void ButtonSymbol_Click(object sender, EventArgs e)
        {
            var text = (sender as Button).Text;
            int carret;

            if (!string.IsNullOrEmpty(_textBoxPath.SelectedText))
            {
                carret = _textBoxPath.Selection.Start;
                _textBoxPath.Text = _textBoxPath.Text.Remove(carret, _textBoxPath.Selection.End + 1 - carret);
            }
            else
                carret = _textBoxPath.CaretIndex;

            _textBoxPath.Text = _textBoxPath.Text.Insert(carret, text);
            _textBoxPath.Focus();
            _textBoxPath.CaretIndex = carret + text.Length;
        }

        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Ok;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
