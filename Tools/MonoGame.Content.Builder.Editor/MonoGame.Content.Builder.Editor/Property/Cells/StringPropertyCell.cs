// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    class StringPropertyCell : PropertyCell
    {
        public override Control Edit()
        {
            var textBox = new TextBox();
            textBox.Text = (Value ?? "").ToString();

            textBox.Focus();
            textBox.CaretIndex = textBox.Text.Length;
            
            textBox.TextChanged += (o, e) => Value = textBox.Text;
            textBox.KeyDown += (sender, e) =>
            {
                if (e.Key == Keys.Enter)
                {
                    (textBox.Parent as PixelLayout)?.Remove(textBox);
                }
            };

            return textBox;
        }
    }
}
