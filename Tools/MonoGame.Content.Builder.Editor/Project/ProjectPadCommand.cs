// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Forms;
using MonoGame.Tools.Pipeline;

namespace MonoGame.Content.Builder.Editor.Project
{
    public abstract class ProjectPadCommand : Command
    {
        private List<IProjectItem> _items;
        private List<TreeGridItem> _treeItems;
        private ProjectPad _projectExplorer;

        public ProjectPadCommand()
        {
            _items = new List<IProjectItem>();

            Executed += (o, e) =>
            {
                if (GetIsActive(_items))
                {
                    Clicked(_projectExplorer, _treeItems, _items);
                }
            };
        }

        public bool OnIsActive(ProjectPad projectExplorer, List<IProjectItem> items, List<TreeGridItem> treeItems)
        {
            _projectExplorer = projectExplorer;

            var active = GetIsActive(items);
            _items = items;
            _treeItems = treeItems;
            Enabled = active;

            if (active)
            {
                MenuText = GetName(items);
            }

            return active;
        }

        /// <summary>
        /// Gets the indices which are used to position the menu item inside the context menu.
        /// 
        /// Each group is separated by a separator menu item.
        /// </summary>
        /// <param name="groupIndex">The group index of the menu items. Groups are separated by separators in the context menu.</param>
        /// <param name="index">The index of menu item inside the group.</param>
        /// <returns>Returns the information about the position of the menu item in the context menu.</returns>
        public abstract (int groupIndex, int index) Index { get; }

        public virtual string Category => string.Empty;

        public virtual bool IsInMenu => false;

        public abstract bool GetIsActive(List<IProjectItem> items);

        public abstract string GetName(List<IProjectItem> items);

        public abstract void Clicked(ProjectPad projectExplorer, List<TreeGridItem> treeItems, List<IProjectItem> items);
    }
}
