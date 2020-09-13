// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    public class ReferencesPropertyCell : PropertyCell
    {
        public override void Edit(PixelLayout control, Rectangle rec)
        {

        }

        public override int Draw(Graphics g, Rectangle rec, bool selected)
        {
            var refs = Value as List<string> ?? new List<string>();
            refs = new List<string>(new[] { "asd.dll", "be.dll", "yo.dll" });

            var ypos = rec.Y + 9;

            foreach (var r in refs)
            {
                g.DrawText(
                    font: DrawInfo.TextFont,
                    color: DrawInfo.GetTextColor(selected, !Editable),
                    x: rec.X + 5,
                    y: ypos,
                    text: r
                );

                ypos += DrawInfo.TextHeight + 9;
            }

            return ypos - rec.Y;
        }
    }
}
