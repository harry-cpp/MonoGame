// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eto.Forms;
using MonoGame.Tools.Pipeline;

namespace MonoGame.Content.Builder.Editor.Project
{
    public class LinkExistingFilesCommand : ProjectPadCommand
    {
        public override (int groupIndex, int index) Index => (10, 40);

        public override string Category => "Add";

        public override bool IsInMenu => true;

        public override bool GetIsActive(List<IProjectItem> items)
        {
            return items.Count == 1 && (items[0] is DirectoryItem || items[0] is PipelineProject);
        }

        public override string GetName(List<IProjectItem> items)
        {
            return "Link Existing Files...";
        }

        public override void Clicked(ProjectPad projectExplorer, List<TreeGridItem> treeItems, List<IProjectItem> items)
        {
            var basePath = items[0] is PipelineProject ? string.Empty : items[0].OriginalPath;
            var allFileFilter = new FileFilter("All Files (*.*)", new[] { ".*" });

            var dialog = new OpenFileDialog();
            dialog.Directory = new Uri(PipelineController.Instance.GetFullPath(basePath));
            dialog.MultiSelect = true;
            dialog.Filters.Add(allFileFilter);
            dialog.CurrentFilter = allFileFilter;

            if (dialog.ShowDialog(projectExplorer) != DialogResult.Ok)
                return;

            var dir1 = PipelineController.Instance.GetFullPath(basePath);
            var dir2 = Path.GetDirectoryName(dialog.Filenames.ElementAt(0));
            var isFileInFolder = dir1 == dir2;

            if (isFileInFolder)
            {
                projectExplorer.AddFiles(dialog.Filenames.ToList());
            }
            else
            {
                var destPaths = new List<string>();

                foreach (var filePath in dialog.Filenames)
                {
                    var relativeDestPath = Path.Combine(basePath, Path.GetFileName(filePath));
                    destPaths.Add(PipelineController.Instance.GetFullPath(relativeDestPath));
                }

                projectExplorer.AddFiles(destPaths, dialog.Filenames.ToList());
            }

            projectExplorer.TreeView.ReloadData();
        }
    }
}
