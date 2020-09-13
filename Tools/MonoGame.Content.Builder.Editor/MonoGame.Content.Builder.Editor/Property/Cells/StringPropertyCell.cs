// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    public class StringPropertyCell : PropertyCell
    {
        public override void Edit(PixelLayout control, Rectangle rec)
        {
            var textBox = new TextBox();
            textBox.Tag = this;
            textBox.Style = "OverrideSize";
            textBox.Width = rec.Width;
            textBox.Height = rec.Height;
            textBox.Text = (Value ?? "").ToString();

            control.Add(textBox, rec.X, rec.Y);

            textBox.Focus();
            textBox.CaretIndex = textBox.Text.Length;
            
            textBox.TextChanged += (o, e) => Value = textBox.Text;
            textBox.KeyDown += (sender, e) =>
            {
                if (e.Key == Keys.Enter)
                {
                    control.Remove(textBox);
                }
            };
        }
    }
}
