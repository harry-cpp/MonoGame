// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Forms;

namespace MonoGame.Tools.Pipeline
{
    partial class PropertyGrid : DynamicLayout
    {
        StackLayout subLayout;
        PropertyGridTable propertyTable;

        private void InitializeComponent()
        {
            BeginVertical();

            subLayout = new StackLayout();
            subLayout.Orientation = Orientation.Horizontal;

            Add(subLayout);

            propertyTable = new PropertyGridTable();
            Add(propertyTable);
        }
   }
}

