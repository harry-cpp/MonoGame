// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
using MonoGame.Tools.Pipeline;

namespace MonoGame.Content.Builder.Editor.ProjectView
{
    public partial class NewFileDialog : Dialog
    {
        private TreeGridItem _treeBase;

        public NewFileDialog()
        {
            InitializeComponent();

            _treeBase = new TreeGridItem();
            LoadTemplates();
        }

        private async void LoadTemplates()
        {
            if (Util.IsMac)
            {
                await LoadTemplates(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Resources"));
            }
            else
            {
                await LoadTemplates(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates"));
            }
            await LoadTemplates(Path.Combine(PipelineController.Instance.ProjectLocation, "MGTemplates"));
        }

        private async Task LoadTemplates(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    return;

                var files = Directory.GetFiles(directoryPath, "*.template", SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    var lines = await File.ReadAllLinesAsync(f);
                    if (lines.Length != 5)
                    {
                        // Invalid template, skip it!
                        continue;
                    }

                    var item = new ContentItemTemplate()
                    {
                        Label = lines[0],
                        Icon = lines[1],
                        ImporterName = lines[2],
                        ProcessorName = lines[3],
                        TemplateFile = lines[4],
                    };

                    var fpath = Path.GetDirectoryName(f);
                    item.TemplateFile = Path.GetFullPath(Path.Combine(fpath, item.TemplateFile));
                    item.Icon = Path.GetFullPath(Path.Combine(fpath, item.Icon));

                    var treeItem = new TreeGridItem();
                    treeItem.SetValue(0, (new Bitmap(item.Icon)).WithSize(16, 16));
                    treeItem.SetValue(1, item.Label);

                    _treeBase.Children.Add(treeItem);

                    _treeBase.Children.Sort((s1, s2) =>
                    {
                        var s1str = (s1 as TreeGridItem).GetValue(1).ToString();
                        var s2str = (s2 as TreeGridItem).GetValue(1).ToString();

                        return s1str.CompareTo(s2str);
                    });
                    _treeView.DataStore = _treeBase;
                    _treeView.ReloadData();
                }
            }
            catch { }
        }
    }
}
