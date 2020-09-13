// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.ComponentModel;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    class NumberPropertyCell : PropertyCell
    {
        private TypeConverter _converter;

        public override void Initialize()
        {
            _converter = TypeDescriptor.GetConverter(Value.GetType());
        }

        public override Control Edit()
        {
            var textBox = new TextBox();
            textBox.Text = Value.ToString();

            textBox.Focus();
            textBox.CaretIndex = textBox.Text.Length;
            
            textBox.TextChanged += (o, e) =>
            {
                try
                {
                    Value = _converter.ConvertFrom(textBox.Text);
                }
                catch { }
            };
            textBox.KeyDown += (sender, e) =>
            {
                if (e.Key == Keys.Enter)
                {
                    (textBox.Parent as PixelLayout)?.Remove(textBox);
                }
            };

            return textBox;
        }

        public override int DrawCell(Graphics g, Rectangle rec, string displayValue, bool selected)
        {
            var type = Value.GetType();
            if (type == typeof(float))
                displayValue = ((float)Value).ToString("0.00");
            else if (type == typeof(double))
                displayValue = ((double)Value).ToString("0.00");
            else if (type == typeof(decimal))
                displayValue = ((decimal)Value).ToString("0.00");

            return base.DrawCell(g, rec, displayValue, selected);
        }
    }
}
