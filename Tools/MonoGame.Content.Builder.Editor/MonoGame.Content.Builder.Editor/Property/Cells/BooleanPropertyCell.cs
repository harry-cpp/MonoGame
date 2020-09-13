// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    public class BooleanPropertyCell : PropertyCell
    {
        public override Control Edit()
        {
            var combo = new DropDown();
            combo.Items.Add("True");
            combo.Items.Add("False");
            combo.SelectedIndex = (bool)Value ? 0 : 1;

            combo.SelectedIndexChanged += (o, e) => Value = combo.SelectedIndex == 0;

            return combo;
        }
    }
}
