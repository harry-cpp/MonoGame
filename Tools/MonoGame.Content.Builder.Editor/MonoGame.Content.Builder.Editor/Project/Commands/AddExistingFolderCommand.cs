// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.IO;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Project
{
    public class AddExistingFolderCommand : ProjectPadCommand
    {
        public override (int groupIndex, int index) Index => (10, 30);

        public override string Category => "Add";

        public override bool IsInMenu => true;

        public override bool GetIsActive(List<IProjectItem> items)
        {
            return items.Count == 1 && (items[0] is DirectoryItem || items[0] is PipelineProject);
        }

        public override string GetName(List<IProjectItem> items)
        {
            return "Add Existing Folder...";
        }

        public override void Clicked(ProjectPad projectPad, List<TreeGridItem> treeItems, List<IProjectItem> items)
        {
            var basePath = items[0] is PipelineProject ? string.Empty : items[0].OriginalPath;
            var dialog = new SelectFolderDialog();
            dialog.Title = "Select Folder to Import";
            dialog.Directory = projectPad.GetFullPath(basePath);

            var result = dialog.Show() == DialogResult.Ok;
            var dirName = Path.GetFileName(dialog.Directory);
            var dirItem = new DirectoryItem(basePath, dirName);

            var treeItem = projectPad.AddItem(treeItems[0], dirItem, dirName);
            ProcessDirectory(projectPad, treeItem, dirItem, dialog.Directory);
            projectPad.TreeView.ReloadData();
        }

        private void ProcessDirectory(ProjectPad projectExplorer, TreeGridItem baseTreeItem, IProjectItem basseItem, string dirPath)
        {
            var basePath = basseItem is PipelineProject ? string.Empty : basseItem.OriginalPath;
            var directories = Directory.GetDirectories(dirPath);

            foreach (var dir in directories)
            {
                var dirName = Path.GetFileName(dir);
                var dirItem = new DirectoryItem(basePath, dirName);

                var treeItem = projectExplorer.AddItem(baseTreeItem, dirItem, dirName);
                ProcessDirectory(projectExplorer, treeItem, dirItem, dir);
            }

            var files = Directory.GetFiles(dirPath);
            
            foreach (var file in files)
            {
                var contentItem = new ContentItem();
                contentItem.OriginalPath = basePath + Path.GetFileName(file);
                contentItem.DestinationPath = basePath + Path.GetFileName(file);
                contentItem.ProcessorParams = new Microsoft.Xna.Framework.Content.Pipeline.OpaqueDataDictionary();
                contentItem.ResolveTypes();

                projectExplorer.AddItem(baseTreeItem, contentItem, Path.GetFileName(file));
            }
        }
    }
}
