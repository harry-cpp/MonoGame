// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Diagnostics;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Project
{
    public class OpenCommand : ProjectPadCommand
    {
        public override (int groupIndex, int index) Index => (0, 0);

        public override bool GetIsActive(List<IProjectItem> items)
        {
            return items.Count == 1 && items[0] is ContentItem;
        }

        public override string GetName(List<IProjectItem> items)
        {
            return "Open \"" + items[0].Name + "\"";
        }

        public override void Clicked(ProjectPad projectPad, List<TreeGridItem> treeItems, List<IProjectItem> items)
        {
            var filePath = projectPad.GetFullPath(items[0].OriginalPath);

            if (Util.IsMac)
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
