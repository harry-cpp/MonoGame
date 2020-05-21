// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Forms;
using MonoGame.Tools.Pipeline;

namespace MonoGame.Content.Builder.Editor.ProjectView
{
    public abstract class ProjectExplorerCommand : Command
    {
        private List<IProjectItem> _items;
        private List<TreeGridItem> _treeItems;
        private ProjectExplorer _projectExplorer;

        public ProjectExplorerCommand()
        {
            _items = new List<IProjectItem>();

            Executed += (o, e) =>
            {
                if (GetIsActive(_items))
                    Clicked(_projectExplorer, _treeItems, _items);
            };
        }

        public void Init(ProjectExplorer projectExplorer)
        {
            _projectExplorer = projectExplorer;
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

        public bool CheckIsActive(List<IProjectItem> items, List<TreeGridItem> treeItems)
        {
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

        public abstract bool GetIsActive(List<IProjectItem> items);

        public abstract string GetName(List<IProjectItem> items);

        public abstract void Clicked(ProjectExplorer projectExplorer, List<TreeGridItem> treeItems, List<IProjectItem> items);
    }
}
