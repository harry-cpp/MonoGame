// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Diagnostics;
using Eto.Forms;
using MonoGame.Tools.Pipeline;

namespace MonoGame.Content.Builder.Editor.ProjectView
{
    public class RevealCommand : ProjectExplorerCommand
    {
        public override (int groupIndex, int index) Index => (20, 0);

        public override bool GetIsActive(List<IProjectItem> items)
        {
            return items.Count == 1 && (items[0] is DirectoryItem || items[0] is ContentItem);
        }

        public override string GetName(List<IProjectItem> items)
        {
            if (!Global.Unix)
            {
                return "Show in Explorer";
            }
            else if (Global.Linux)
            {
                return "Open Containing Folder";
            }
            else
            {
                return "Reveal in Finder";
            }
        }

        public override void Clicked(ProjectExplorer projectExplorer, List<TreeGridItem> treeItems, List<IProjectItem> items)
        {
            var filePath = PipelineController.Instance.GetFullPath(items[0].Location);

            if (Global.IsMac)
            {
                Process.Start("open", filePath);
            }
            else
            {
                Process.Start(filePath);
            }
        }
    }
}
