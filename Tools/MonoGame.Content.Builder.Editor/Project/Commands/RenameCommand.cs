// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Forms;
using MonoGame.Tools.Pipeline;

namespace MonoGame.Content.Builder.Editor.Project
{
    public class RenameCommand : ProjectPadCommand
    {
        public override (int groupIndex, int index) Index => (30, 0);

        public override bool IsInMenu => true;

        public override bool GetIsActive(List<IProjectItem> items)
        {
            return items.Count == 1 && !(items[0] is PipelineProject);
        }

        public override string GetName(List<IProjectItem> items)
        {
            return "Rename";
        }

        public override void Clicked(ProjectPad projectExplorer, List<TreeGridItem> treeItems, List<IProjectItem> items)
        {
            projectExplorer.TreeView.BeginEdit(projectExplorer.TreeView.SelectedRow, 0);
        }
    }
}
