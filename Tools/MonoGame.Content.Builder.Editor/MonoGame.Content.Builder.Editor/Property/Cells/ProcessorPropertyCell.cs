// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    class ProcessorPropertyCell : PropertyCell
    {
        public override string DisplayValue => (Value as ProcessorTypeDescription).DisplayName;

        public override Control Edit()
        {
            var combo = new DropDown();

            var values = PipelineTypes.Processors;
            Type inputType = null;
            
            // Find the input type
            foreach (var v in PipelineTypes.Processors)
            {
                if (Value != null && v.DisplayName == (Value as ProcessorTypeDescription).DisplayName)
                {
                    inputType = v.InputType;
                    break;
                }
            }

            // Load entries to dropdown only if they match the input type
            foreach (var v in PipelineTypes.Processors)
            {
                if (inputType == null || v.InputType == inputType)
                {
                    combo.Items.Add(v.DisplayName);

                    if (Value != null && v.DisplayName == (Value as ProcessorTypeDescription).DisplayName)
                        combo.SelectedIndex = combo.Items.Count - 1;
                }
            }

            combo.SelectedIndexChanged += (o, e) => Value = values.GetValue(combo.SelectedIndex);

            return combo;
        }
    }
}
