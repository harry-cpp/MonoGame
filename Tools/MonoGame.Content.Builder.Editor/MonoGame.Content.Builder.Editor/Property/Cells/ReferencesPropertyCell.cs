// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.IO;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    class ReferencesPropertyCell : PropertyCell
    {
        public override Control Edit()
        {
            var basePath = string.Empty;
            if (ParentObjects.Count > 0 && ParentObjects[0] is PipelineProject projectItem)
                basePath = projectItem.Location;

            var references = Value as List<string> ?? new List<string>();
            var referencesDialog = new ReferencesDialog(basePath, references);

            if (referencesDialog.Show() == DialogResult.Ok)
                Value = referencesDialog.References;

            return null;
        }

        public override int DrawCell(Graphics g, Rectangle rec, string displayValue, bool selected)
        {
            var references = new List<string>((Value as List<string> ?? new List<string>()));
            for (int i = 0; i < references.Count; i++)
                references[i] = Path.GetFileNameWithoutExtension(references[i]);
            references.Sort();

            var y = rec.Y + (DrawInfo.Spacing / 2);
            var height = DrawInfo.TextHeight + (DrawInfo.Spacing / 2);

            foreach (var r in references)
            {
                g.DrawText(
                    font: DrawInfo.TextFont,
                    color: DrawInfo.GetTextColor(selected, !Editable),
                    x: rec.X + 5,
                    y: y,
                    text: r
                );

                y += height;
            }

            if (references.Count == 0)
                y += height;

            return y - rec.Y;
        }
    }
}
