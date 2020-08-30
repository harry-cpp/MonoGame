// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Forms;
using MonoGame.Tools.Pipeline;

namespace MonoGame.Content.Builder.Editor.Project
{
    public partial class ProjectPad : Pad
    {
        private TreeGridView _treeView;

        private void InitializeComponent()
        {
            Title = "Project Explorer";

            _treeView = new TreeGridView();
            _treeView.ContextMenu = new ContextMenu();
            _treeView.ShowHeader = false;
            _treeView.AllowMultipleSelection = true;
            _treeView.Columns.Add(new GridColumn
            {
                DataCell = new ImageTextCell(0, 1),
                Editable = true
            });

            SetMainContent(_treeView);
        }
    }
}
