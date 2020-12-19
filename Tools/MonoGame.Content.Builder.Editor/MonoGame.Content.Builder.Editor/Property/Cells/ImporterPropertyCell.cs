// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    class ImporterPropertyCell : PropertyCell
    {
        public override Control Edit()
        {
            var combo = new DropDown();
            var values = PipelineTypes.Importers;
            foreach (var v in values)
            {
                combo.Items.Add(v.DisplayName);

                if (Value is ImporterTypeDescription importer && v.DisplayName == importer.DisplayName)
                    combo.SelectedIndex = combo.Items.Count - 1;
            }

            combo.SelectedIndexChanged += (o, e) =>
            {
                Value = values.GetValue(combo.SelectedIndex);
            };

            return combo;
        }

        public override int DrawCell(Graphics g, Rectangle rec, string displayValue, bool selected)
        {
            if (Value is ImporterTypeDescription importer)
                displayValue = importer.DisplayName;

            return base.DrawCell(g, rec, displayValue, selected);
        }
    }
}
