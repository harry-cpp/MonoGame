// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Forms;
using MonoGame.Tools.Pipeline;

namespace MonoGame.Content.Builder.Editor.Project
{
    public class RebuildCommand : ProjectPadCommand
    {
        public override (int groupIndex, int index) Index => (40, 10);

        public override bool GetIsActive(List<IProjectItem> items)
        {
            return true;
        }

        public override string GetName(List<IProjectItem> items)
        {
            return "Rebuild";
        }

        public override void Clicked(ProjectPad projectExplorer, List<TreeGridItem> treeItems, List<IProjectItem> items)
        {

        }
    }
}
