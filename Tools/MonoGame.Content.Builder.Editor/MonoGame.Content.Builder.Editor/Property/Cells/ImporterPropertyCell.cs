// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    class ImporterPropertyCell : PropertyCell
    {
        public override string DisplayValue => (Value as ImporterTypeDescription).DisplayName;

        public override Control Edit()
        {
            var combo = new DropDown();

            var values = PipelineTypes.Importers;
            foreach (var v in values)
            {
                combo.Items.Add(v.DisplayName);

                if (Value != null && v.DisplayName == (Value as ImporterTypeDescription).DisplayName)
                    combo.SelectedIndex = combo.Items.Count - 1;
            }

            combo.SelectedIndexChanged += (o, e) => Value = values.GetValue(combo.SelectedIndex);

            return combo;
        }
    }
}
