// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Globalization;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;

namespace MonoGame.Content.Builder.Editor.Property
{
    public class ColorDialog : Dialog<DialogResult>
    {
        private TextBox _textBoxRed = null;
        private TextBox _textBoxGreen = null;
        private TextBox _textBoxBlue = null;
        private TextBox _textBoxAlpha = null;
        private TextBox _textBoxHex = null;
        private ColorPicker _colorPicker = null;

        private bool _eventsDisabled;
        private Color _color;

        public ColorDialog()
        {
            XamlReader.Load(this, "MonoGame.Content.Builder.Editor.Property.Dialogs.ColorDialog.xeto");

            _eventsDisabled = false;
            _color = new Color();
        }

        public Color Color
        {
            get => _color;
            set
            {
                _eventsDisabled = true;

                _color = value;

                _textBoxRed.Text = value.Rb.ToString();
                _textBoxGreen.Text = value.Gb.ToString();
                _textBoxBlue.Text = value.Bb.ToString();
                _textBoxAlpha.Text = value.Ab.ToString();

                _textBoxHex.Text = value.ToHex(false) + value.Ab.ToString("X2");

                _colorPicker.Value = value;

                _eventsDisabled = false;
            }
        }

        private void TextBoxRGBA_TextChanged(object sender, EventArgs e)
        {
            if (_eventsDisabled)
                return;

            try
            {
                Color = new Color(
                    byte.Parse(_textBoxRed.Text) / 255f,
                    byte.Parse(_textBoxGreen.Text) / 255f,
                    byte.Parse(_textBoxBlue.Text) / 255f,
                    byte.Parse(_textBoxAlpha.Text) / 255f
                );
            }
            catch { }
        }

        private void TextBoxHex_TextChanged(object sender, EventArgs e)
        {
            if (_eventsDisabled)
                return;

            if (_textBoxHex.Text.Length == 9)
            {
                try
                {
                    var color = Color.Parse(_textBoxHex.Text.Substring(0, 7));
                    color.Ab = int.Parse(_textBoxHex.Text.Substring(7, 2), NumberStyles.HexNumber);
                    Color = color;
                }
                catch { }
            }
        }

        private void ColorPicker_ValueChanged(object sender, EventArgs e)
        {
            if (_eventsDisabled)
                return;

            Color = _colorPicker.Value;
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
