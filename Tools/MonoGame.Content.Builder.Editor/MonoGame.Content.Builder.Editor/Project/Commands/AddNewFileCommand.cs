// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Project
{
    public class AddNewFileCommand : ProjectPadCommand
    {
        public override (int groupIndex, int index) Index => (10, 0);

        public override string Category => "Add";

        public override bool IsInMenu => true;

        public override bool GetIsActive(List<IProjectItem> items)
        {
            return items.Count == 1 && (items[0] is DirectoryItem || items[0] is PipelineProject);
        }

        public override string GetName(List<IProjectItem> items)
        {
            return "Add New File...";
        }

        public override async void Clicked(ProjectPad projectPad, List<TreeGridItem> treeItems, List<IProjectItem> items)
        {
            var dialog = new NewFileDialog();
            await dialog.ShowModalAsync();
        }
    }
}
