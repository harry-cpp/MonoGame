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

        public override string DisplayValue
        {
            get
            {
                var type = Value.GetType();
                if (type == typeof(float))
                    return ((float)Value).ToString("0.00");
                else if(type == typeof(double))
                    return ((double)Value).ToString("0.00");
                else if (type == typeof(decimal))
                    return ((decimal)Value).ToString("0.00");

                return Value.ToString();
            }
        }

        public override void Initialize()
        {
            _converter = TypeDescriptor.GetConverter(Value.GetType());
        }

        public override void Edit(PixelLayout control, Rectangle rec)
        {
            var textBox = new TextBox();
            textBox.Width = rec.Width;
            textBox.Height = rec.Height;
            textBox.Text = Value.ToString();

            control.Add(textBox, rec.X, rec.Y);

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
                    control.Remove(textBox);
                }
            };
        }
    }
}
