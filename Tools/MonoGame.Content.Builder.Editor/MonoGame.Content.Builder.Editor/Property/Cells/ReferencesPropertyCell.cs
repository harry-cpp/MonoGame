// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    class ReferencesPropertyCell : PropertyCell
    {
        public override Control Edit()
        {
            return null;
        }

        public override int DrawCell(Graphics g, Rectangle rec, string displayValue, bool selected)
        {
            var refs = Value as List<string> ?? new List<string>();
            refs = new List<string>(new[] { "asd.dll", "be.dll", "yo.dll" });

            var ypos = rec.Y + 6;

            foreach (var r in refs)
            {
                g.DrawText(
                    font: DrawInfo.TextFont,
                    color: DrawInfo.GetTextColor(selected, !Editable),
                    x: rec.X + 5,
                    y: ypos,
                    text: r
                );

                ypos += DrawInfo.TextHeight + 6;
            }

            return ypos - rec.Y;
        }
    }
}
