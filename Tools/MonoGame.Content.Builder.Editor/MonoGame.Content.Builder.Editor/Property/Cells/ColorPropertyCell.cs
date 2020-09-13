// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    public class ColorPropertyCell : PropertyCell
    {
        private Color _color;

        public override void Initialize()
        {
            var xnaColor = (Microsoft.Xna.Framework.Color)Value;
            _color = new Color(xnaColor.R / 255f, xnaColor.G / 255f, xnaColor.B / 255f, xnaColor.A / 255f);
        }

        public override void Edit(PixelLayout control, Rectangle rec)
        {
            var dialog = new ColorDialog();
            dialog.Color = _color;

            if (dialog.Show() == DialogResult.Ok)
            {
                _color = dialog.Color;
                Value = new Microsoft.Xna.Framework.Color(dialog.Color.Rb, dialog.Color.Gb, dialog.Color.Bb, dialog.Color.Ab);
            }
        }

        private Color GetContrastColor(Color color)
        {
            var luminance = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;

            if (luminance > 0.5)
                return Colors.Black;
            else
                return Colors.White;
        }

        public override int Draw(Graphics g, Rectangle rec, bool selected)
        {
            g.FillRectangle(_color, new Rectangle(rec.X + 4, rec.Y + 4, rec.Width - 8, rec.Height - 8));
            g.DrawText(
                font: DrawInfo.TextFont,
                color: GetContrastColor(_color),
                x: rec.X + 5,
                y: rec.Y + 9,
                text: " " + _color.ToHex()
            );

            return rec.Height;
        }
    }
}
