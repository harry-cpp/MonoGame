// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Project
{
    public partial class NewFileDialog : Dialog
    {
        private Button _buttonCreate, _buttonCancel;
        private TreeGridView _treeView;
        private TextBox _textBox;
        private TreeGridItem _treeBase;

        public NewFileDialog()
        {
            DisplayMode =  DialogDisplayMode.Attached;
            Resizable = false;
            Size = new Size(300, 400);
            Title = "New File";
            Padding = 8;

            var dynamicLayout = new DynamicLayout();
            dynamicLayout.Spacing = new Eto.Drawing.Size(8, 8);

            _textBox = new TextBox();
            

            _treeView = new TreeGridView();
            _treeView.AllowEmptySelection = false;
            _treeView.AllowMultipleSelection = false;
            _treeView.Columns.Add(new GridColumn
            {
                DataCell = new ImageTextCell(0, 1),
                HeaderText = "Templates"
            });

            // On Linux dialog buttons are on top
            if (Util.IsLinux)
            {
                dynamicLayout.Add(_textBox);
                dynamicLayout.Add(_treeView, true, true);
            }
            else
            {
                dynamicLayout.Add(_treeView, true, true);
                dynamicLayout.Add(_textBox);
            }

            Content = dynamicLayout;

            _buttonCreate = new Button();
            _buttonCreate.Text = "Create File";
            PositiveButtons.Add(_buttonCreate);
            DefaultButton = _buttonCreate;

            _buttonCancel = new Button();
            _buttonCancel.Text = "Cancel";
            NegativeButtons.Add(_buttonCancel);
            AbortButton = _buttonCancel;

            _treeBase = new TreeGridItem();
            LoadTemplates();
        }

        private async void LoadTemplates()
        {
            if (Util.IsXamarinMac)
            {
                await LoadTemplates(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Resources"));
            }
            else
            {
                await LoadTemplates(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates"));
            }
            // await LoadTemplates(Path.Combine(Controller.ProjectLocation, "MGTemplates"));
        }

        private async Task LoadTemplates(string directoryPath)
        {
            /*try
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
                        if (s1 is TreeGridItem s1i && s2 is TreeGridItem s2i)
                        {
                            var s1str = s1i.GetValue(1).ToString();
                            var s2str = s2i.GetValue(1).ToString();

                            return s1str.CompareTo(s2str);
                        }

                        return 0;
                    });
                    _treeView.DataStore = _treeBase;
                    _treeView.ReloadData();
                }
            }
            catch { }*/
        }
    }
}
