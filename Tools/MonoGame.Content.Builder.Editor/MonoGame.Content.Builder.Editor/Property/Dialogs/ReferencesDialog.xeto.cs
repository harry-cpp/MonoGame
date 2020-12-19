// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using Eto.Forms;
using Eto.Serialization.Xaml;

namespace MonoGame.Content.Builder.Editor.Property
{
    public class ReferencesDialog : Dialog<DialogResult>
    {
        private TreeGridView _treeView = null;
        private Button _buttonRemove = null;

        private string _basePath;
        private List<string> _references;
        private TreeGridItem _itemBase;

        public ReferencesDialog(string basePath, List<string> references)
        {
            XamlReader.Load(this, "MonoGame.Content.Builder.Editor.Property.Dialogs.ReferencesDialog.xeto");

            _basePath = basePath;
            _references = references;
            _itemBase = new TreeGridItem();

            _treeView.Columns[0].DataCell = new TextBoxCell(0);
            _treeView.Columns[1].DataCell = new TextBoxCell(1);

            foreach (var reference in references)
            {
                var item = new TreeGridItem();
                item.SetValue(0, Path.GetFileNameWithoutExtension(reference));
                item.SetValue(1, reference);

                _itemBase.Children.Add(item);
            }

            _treeView.DataStore = _itemBase;
            _treeView.ReloadData();

            _buttonRemove.Enabled = _treeView.SelectedItem as TreeGridItem != null;
        }

        public List<string> References => _references;

        private void TreeView_SelectedItemsChanged(object sender, EventArgs e)
        {
            _buttonRemove.Enabled = _treeView.SelectedItem as TreeGridItem != null;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Directory = new Uri(_basePath);
            dialog.MultiSelect = true;
            dialog.Filters.Add(new FileFilter("Dll Files (*.dll)", new[] { ".dll" }));
            dialog.Filters.Add(new FileFilter("All Files (*.*)", new[] { ".*" }));

            if (dialog.Show() == DialogResult.Ok)
            {
                foreach (var filePath in dialog.Filenames)
                {
                    var item = new TreeGridItem();
                    item.SetValue(0, Path.GetFileNameWithoutExtension(filePath));
                    item.SetValue(1, Path.GetRelativePath(_basePath, filePath));

                    _itemBase.Children.Add(item);
                }

                _treeView.DataStore = _itemBase;
                _treeView.ReloadData();
            }
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            if (_treeView.SelectedItem is TreeGridItem selectedItem)
            {
                _itemBase.Children.Remove(selectedItem);
                _treeView.DataStore = _itemBase;
                _treeView.ReloadData();
            }
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            _references.Clear();

            foreach (var referenceItem in _itemBase.Children)
                if (referenceItem is TreeGridItem item)
                    _references.Add(item.GetValue(1).ToString());
            _references.Sort();

            Result = DialogResult.Ok;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
