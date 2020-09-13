// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    public class ImporterPropertyCell : PropertyCell
    {
        public override string DisplayValue => (Value as ImporterTypeDescription).DisplayName;

        public override void Edit(PixelLayout control, Rectangle rec)
        {
            var combo = new DropDown();
            combo.Width = rec.Width;
            combo.Height = rec.Height;

            var values = PipelineTypes.Importers;
            foreach (var v in values)
            {
                combo.Items.Add(v.DisplayName);

                if (Value != null && v.DisplayName == (Value as ImporterTypeDescription).DisplayName)
                    combo.SelectedIndex = combo.Items.Count - 1;
            }

            control.Add(combo, rec.X, rec.Y);

            combo.SelectedIndexChanged += (o, e) => Value = values.GetValue(combo.SelectedIndex);
        }
    }
}
