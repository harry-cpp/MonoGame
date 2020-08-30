// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Project
{
    public class OpenWithCommand : ProjectPadCommand
    {
        public override (int groupIndex, int index) Index => (0, 10);

        public override bool GetIsActive(List<IProjectItem> items)
        {
            return items.Count == 1 && items[0] is ContentItem;
        }

        public override string GetName(List<IProjectItem> items)
        {
            return "Open With...";
        }

        public override void Clicked(ProjectPad projectExplorer, List<TreeGridItem> treeItems, List<IProjectItem> items)
        {
            var filepath = Controller.GetFullPath(items[0].OriginalPath);
            var dialog = new OpenWithDialog(filepath);
            dialog.ShowDialog(projectExplorer);
        }
    }
}
