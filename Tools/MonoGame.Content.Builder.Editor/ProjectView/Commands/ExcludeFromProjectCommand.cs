// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Forms;
using MonoGame.Tools.Pipeline;

namespace MonoGame.Content.Builder.Editor.ProjectView
{
    public class ExcludeFromProjectCommand : ProjectExplorerCommand
    {
        public override (int groupIndex, int index) Index => (30, 10);

        public override bool GetIsActive(List<IProjectItem> items)
        {
            foreach (var item in items)
            {
                if (item is PipelineProject)
                {
                    return false;
                }
            }

            return true;
        }

        public override string GetName(List<IProjectItem> items)
        {
            return "Exclude From Project";
        }

        public override void Clicked(ProjectExplorer projectExplorer, List<TreeGridItem> treeItems, List<IProjectItem> items)
        {
            foreach (var treeItem in treeItems)
            {
                if (treeItem.Parent is TreeGridItem parent)
                {
                    parent.Children.Remove(treeItem);
                }
            }

            projectExplorer.TreeView.ReloadData();
        }
    }
}
