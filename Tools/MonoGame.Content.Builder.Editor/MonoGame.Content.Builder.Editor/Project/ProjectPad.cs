// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using OpaqueDataDictionary = Microsoft.Xna.Framework.Content.Pipeline.OpaqueDataDictionary;

namespace MonoGame.Content.Builder.Editor.Project
{
    public partial class ProjectPad : Pad
    {
        private IController _controller;
        private PipelineProject _project;
        private TreeGridItem _itemBase, _itemRoot;
        private Image _iconRoot;
        private List<ProjectPadCommand> _commands;
        private TreeGridView _treeView;
        private ContextMenu _contextMenu;

        public ProjectPad(IController controller)
        {
            _controller = controller;

            _treeView = new TreeGridView();
            _treeView.ShowHeader = false;
            _treeView.AllowMultipleSelection = true;
            _treeView.Columns.Add(new GridColumn
            {
                DataCell = new ImageTextCell(0, 1),
                Editable = true
            });

            _contextMenu = new ContextMenu();

            _itemBase = new TreeGridItem();
            _itemRoot = new TreeGridItem();
            _iconRoot = Bitmap.FromResource("TreeView.Root.png").WithSize(16, 16);

            _commands = new List<ProjectPadCommand>();

            foreach (var t in typeof(ProjectPad).Assembly.GetTypes())
            {
                if (!t.IsAbstract && typeof(ProjectPadCommand).IsAssignableFrom(t))
                {
                    var cmd = (ProjectPadCommand)Activator.CreateInstance(t);
                    cmd.Init(this);
                    cmd.OnIsActive(new List<IProjectItem>(), new List<TreeGridItem>());

                    _commands.Add(cmd);
                }
            }

            _commands.Sort((ProjectPadCommand x, ProjectPadCommand y) =>
            {
                var xindex = x.Index;
                var yindex = y.Index;

                if (xindex.groupIndex != yindex.groupIndex)
                    return xindex.groupIndex - yindex.groupIndex;

                return xindex.index - yindex.index;
            });

            /*foreach (var cmd in _commands)
            {
                if (cmd.IsInMenu)
                {
                    AddMenuItem("Edit/" + cmd.Category + "/" + cmd.GetName(new List<IProjectItem>()), cmd);
                }
            }*/

            _treeView.CellEditing += TreeView_CellEditing;
            _treeView.CellEdited += TreeView_CellEdited;
            _treeView.SelectionChanged += TreeView_SelectionChanged;

            // Workarond for the Eto.Forms Gtk backend not properly resizing
            // the context menu to fit all of the items in it.
            if (Util.IsGtk)
            {
                _treeView.MouseUp += (o, e) =>
                {
                    if (e.Buttons == MouseButtons.Alternate)
                    {
                        _contextMenu.Show();
                    }
                };
            }
            else
            {
                _treeView.ContextMenu = _contextMenu;
            }
        }

        public TreeGridView TreeView => _treeView;

        public TreeGridItem TreeRoot => _itemRoot;

        public override Control Control => _treeView;

        public override string Title => "Project Explorer";

        public override void UpdateEnabledCommands(Commands commands)
        {
            
        }

        public string GetFullPath(string filePath)
        {
            if (_project == null || Path.IsPathRooted(filePath))
            {
                if (filePath.Length == 2 && filePath[0] != '/')
                    filePath += "\\";
                return filePath;
            }

            filePath = filePath.Replace("/", Path.DirectorySeparatorChar.ToString());
            if (filePath.StartsWith("\\"))
                filePath = filePath.Substring(1);

            return _project.Location + Path.DirectorySeparatorChar + filePath;
        }

        public string GetRelativePath(string path)
        {
            if (_project == null)
                return path;

            var dirUri = new Uri(_project.Location);
            var fileUri = new Uri(path);
            var relativeUri = dirUri.MakeRelativeUri(fileUri);

            if (relativeUri == null)
                return path;

            return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        private void TreeView_CellEditing(object sender, GridViewCellEventArgs e)
        {
            if (e.Item is TreeGridItem item && item.GetValue(2) is PipelineProject)
                _treeView.CancelEdit();
        }

        private async void TreeView_CellEdited(object sender, GridViewCellEventArgs e)
        {
            var item = e.Item as TreeGridItem;

            if (item == null)
                return;

            var newFileName = item.GetValue(1).ToString();
            var projectItem = item.GetValue(2) as IProjectItem;

            if (projectItem is PipelineProject || projectItem == null || projectItem.Name == newFileName)
                return;

            // If original path is the same as the destionation path,
            // than the item is not a link, so we should move the source file.
            if (projectItem.OriginalPath == projectItem.DestinationPath)
            {
                var progressDialog = new FileProgressDialog(() =>
                {
                    var location = GetFullPath(projectItem.Location);
                    var originalPath = Path.Combine(location, projectItem.Name);
                    var newPath = Path.Combine(location, newFileName);

                    if (projectItem is ContentItem)
                        File.Move(originalPath, newPath);
                    else if (projectItem is DirectoryItem)
                        Directory.Move(originalPath, newPath);
                    else
                        throw new Exception("How did this happen?");
                });

                await progressDialog.ShowAsync();

                if (!progressDialog.IsSuccess)
                {
                    item.SetValue(1, projectItem.Name);
                    _treeView.ReloadData();
                    return;
                }

                projectItem.OriginalPath = projectItem.Location + "/" + newFileName;
            }

            projectItem.DestinationPath = projectItem.Location + "/" + newFileName;
        }

        private void TreeView_SelectionChanged(object sender, EventArgs e)
        {
            if (_project == null)
                return;

            var items = new List<IProjectItem>();
            var treeItems = new List<TreeGridItem>();
            var dic = new Dictionary<string, MenuItemCollection>();

            if (_treeView.SelectedItems != null)
            {
                foreach (TreeGridItem selected in _treeView.SelectedItems)
                {
                    if (selected.GetValue(2) is IProjectItem item)
                        items.Add(item);

                    treeItems.Add(selected);
                }
            }

            // Populate property grid

            _controller.LoadProperties(items);

            // Populate context menu

            _contextMenu.Items.Clear();

            var lastGroupNum = -1;

            foreach (var item in _commands)
            {
                if (!item.OnIsActive(items, treeItems))
                    continue;

                // Add menu separator

                var groupIndex = item.Index.groupIndex;
                if (groupIndex > lastGroupNum)
                {
                    if (lastGroupNum != -1)
                        _contextMenu.Items.Add(new SeparatorMenuItem());
                    lastGroupNum = groupIndex;
                }

                // Add menu item

                if (!string.IsNullOrWhiteSpace(item.Category))
                {
                    if (!dic.TryGetValue(item.Category, out MenuItemCollection menu))
                    {
                        var buttonMenuItem = new ButtonMenuItem();
                        buttonMenuItem.Text = item.Category;
                        _contextMenu.Items.Add(buttonMenuItem);

                        dic[item.Category] = buttonMenuItem.Items;
                    }

                    dic[item.Category].Add(item.CreateMenuItem());
                }
                else
                {
                    _contextMenu.Items.Add(item.CreateMenuItem());
                }
            }
        }

        public void Open(PipelineProject project)
        {
            _project = project;
            _itemBase = new TreeGridItem();

            if (project != null)
            {
                _itemRoot = new TreeGridItem();
                _itemRoot.Expanded = true;
                _itemRoot.SetValue(0, _iconRoot);
                _itemRoot.SetValue(1, project.Name);
                _itemRoot.SetValue(2, project);
                _itemBase.Children.Add(_itemRoot);

                foreach (var contentItem in project.ContentItems)
                    AddItem(_itemRoot, contentItem, contentItem.DestinationPath);
            }

            _treeView.DataStore = _itemBase;
            _treeView.ReloadData();
        }

        public TreeGridItem AddItem(TreeGridItem root, IProjectItem projectItem, string filePath)
        {
            var split = filePath.Split('/', '\\');
            var rootItemPath = string.Empty;

            if (root.GetValue(2) is IProjectItem item && !(item is PipelineProject))
                rootItemPath = item.OriginalPath;

            var findItem = root.Children.FirstOrDefault(p =>
                p is TreeGridItem pitem &&
                pitem.GetValue(1).ToString() == split[0]
            ) as TreeGridItem;

            if (findItem == null)
            {
                findItem = new TreeGridItem();
                findItem.SetValue(1, split[0]);

                if (split.Length == 1 && projectItem is ContentItem)
                {
                    var originalFilePath = GetFullPath(projectItem.OriginalPath);
                    var link = projectItem.OriginalPath != projectItem.DestinationPath;

                    findItem.SetValue(0, _controller.GetFileIcon(originalFilePath, link));
                    findItem.SetValue(2, projectItem);
                }
                else
                {
                    findItem.SetValue(0, _controller.GetFolderIcon());
                    findItem.SetValue(2, split.Length == 1 ? projectItem : new DirectoryItem(split[0], rootItemPath));
                }

                root.Children.Add(findItem);
                SortItem(root);
            }

            if (split.Length > 1)
                AddItem(findItem, projectItem, string.Join("/", split, 1, split.Length - 1));

            return findItem;
        }

        public void AddFiles(List<string> filePaths)
        {
            AddFiles(filePaths, filePaths);
        }

        public void AddFiles(List<string> sourceFilePaths, List<string> destFilePaths)
        {
            for (int i = 0; i < sourceFilePaths.Count; i++)
            {
                var relativePath = GetRelativePath(destFilePaths[i]);

                if (Directory.Exists(sourceFilePaths[i]))
                {
                    var dirItem = new DirectoryItem(relativePath);

                    AddItem(TreeRoot, dirItem, relativePath);
                }
                else
                {
                    var contentItem = new ContentItem();
                    contentItem.ProcessorParams = new OpaqueDataDictionary();
                    contentItem.OriginalPath = GetRelativePath(sourceFilePaths[i]);
                    contentItem.DestinationPath = relativePath;
                    contentItem.ResolveTypes();

                    AddItem(TreeRoot, contentItem, relativePath);
                }
            }
        }

        private void SortItem(TreeGridItem item)
        {
            item.Children.Sort((s1, s2) =>
            {
                if (s1 is TreeGridItem s1Item && s2 is TreeGridItem s2Item)
                {
                    var s1Dir = s1Item.GetValue(2) is DirectoryItem;
                    var s2Dir = s2Item.GetValue(2) is DirectoryItem;

                    if (s1Dir == s2Dir)
                        return s1Item.GetValue(1).ToString().CompareTo(s2Item.GetValue(1).ToString());

                    return s2Dir ? 1 : -1;
                }

                return 0;
            });
        }
    }
}
