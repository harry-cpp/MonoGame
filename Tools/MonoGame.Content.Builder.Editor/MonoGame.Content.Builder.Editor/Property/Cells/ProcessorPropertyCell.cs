// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    class ProcessorPropertyCell : PropertyCell
    {
        public override Control Edit()
        {
            var combo = new DropDown();

            Type inputType = null;
            if (ParentObjects.Count > 0 && ParentObjects[0] is ContentItem contentItem && contentItem.Processor != null)
                inputType = contentItem.Processor.InputType;

            foreach (var v in PipelineTypes.Processors)
            {
                if (inputType == null || v.InputType == inputType)
                {
                    combo.Items.Add(v.DisplayName);

                    if (Value is ProcessorTypeDescription processor && v.DisplayName == processor.DisplayName)
                        combo.SelectedIndex = combo.Items.Count - 1;
                }
            }

            combo.SelectedIndexChanged += (o, e) =>
            {
                if (combo.SelectedValue is string displayName)
                {
                    Value = PipelineTypes.Processors.ToList().Find((processor) => processor.DisplayName == displayName);
                }
            };

            return combo;
        }

        public override int DrawCell(Graphics g, Rectangle rec, string displayValue, bool selected)
        {
            if (Value is ProcessorTypeDescription processor)
                displayValue = processor.DisplayName;

            return base.DrawCell(g, rec, displayValue, selected);
        }
    }
}
