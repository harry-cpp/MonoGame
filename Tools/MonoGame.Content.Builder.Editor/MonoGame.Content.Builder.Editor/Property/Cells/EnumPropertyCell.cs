// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    class EnumPropertyCell : PropertyCell
    {
        public override Control Edit()
        {
            var combo = new DropDown();

            var values = Enum.GetValues(Value.GetType());
            foreach (var value in values)
            {
                combo.Items.Add(value.ToString());

                if (Value != null && value.ToString() == Value.ToString())
                    combo.SelectedIndex = combo.Items.Count - 1;
            }

            combo.SelectedIndexChanged += (o, e) => Value = values.GetValue(combo.SelectedIndex);

            return combo;
        }
    }
}
