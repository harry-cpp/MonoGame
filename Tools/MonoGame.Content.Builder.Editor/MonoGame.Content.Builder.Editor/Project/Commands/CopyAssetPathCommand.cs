// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.IO;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Project
{
    public class CopyAssetPathCommand : ProjectPadCommand
    {
        public override (int groupIndex, int index) Index => (40, 0);

        public override bool IsInMenu => true;

        public override bool GetIsActive(List<IProjectItem> items)
        {
            return items.Count == 1 && items[0] is ContentItem;
        }

        public override string GetName(List<IProjectItem> items)
        {
            return "Copy Asset Path";
        }

        public override void Clicked(ProjectPad projectPad, List<TreeGridItem> treeItems, List<IProjectItem> items)
        {
            var filePath = items[0].DestinationPath;
            filePath = filePath.Remove(filePath.Length - Path.GetExtension(filePath).Length);
            filePath = filePath.Replace('\\', '/');

            var clipboard = new Clipboard();
            clipboard.Text = filePath;
        }
    }
}
