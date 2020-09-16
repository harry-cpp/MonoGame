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
            var check = new CheckBox();
            check.Checked = (bool?)Value;
            check.Text = Value == null ? "Null" : Value.ToString();
            check.ThreeState = (Value == null);

            check.CheckedChanged += (o, e) => 
            {
                Value = check.Checked;
                check.ThreeState = false;
                check.Text = check.Checked.ToString();
            };

            var box = new DynamicLayout();
            box.DefaultPadding = new Eto.Drawing.Padding(4, 0, 0, 0);
            box.Add(check, true, true);

            return box;
        }
    }
}
