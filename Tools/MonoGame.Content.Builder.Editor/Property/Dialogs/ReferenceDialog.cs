// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    partial class ReferenceDialog : Dialog<bool>
    {
        protected class RefItem
        {
            public string Assembly { get; set; }
            public string Location { get; set; }

            public RefItem(string assembly, string location)
            {
                Assembly = assembly;
                Location = location;
            }
        }

        public List<string> References { get; private set; }

        private FileFilter _dllFileFilter, _allFileFilter;
        private SelectableFilterCollection<RefItem> _dataStore;

        public ReferenceDialog(string[] refs)
        {
            InitializeComponent();

            _dllFileFilter = new FileFilter("Dll Files (*.dll)", new[] { ".dll" });
            _allFileFilter = new FileFilter("All Files (*.*)", new[] { ".*" });

            var assemblyColumn = new GridColumn();
            assemblyColumn.HeaderText = "Assembly";
            assemblyColumn.DataCell = new TextBoxCell("Assembly");
            assemblyColumn.Sortable = true;
            grid1.Columns.Add(assemblyColumn);

            var locationColumn = new GridColumn();
            locationColumn.HeaderText = "Location";
            locationColumn.DataCell = new TextBoxCell("Location");
            locationColumn.Sortable = true;
            grid1.Columns.Add(locationColumn);

            grid1.DataStore = _dataStore = new SelectableFilterCollection<RefItem>(grid1);

            foreach (var rf in refs)
                _dataStore.Add(new RefItem(Path.GetFileName(rf), Tools.Pipeline.PipelineController.Instance.GetFullPath(rf)));

            grid1.SelectionChanged += Grid1_SelectionChanged;
            grid1.KeyDown += Grid1_KeyDown;
            buttonAdd.Click += ButtonAdd_Click;
            buttonRemove.Click += ButtonRemove_Click;
            buttonOk.Click += ButtonOk_Click;
            buttonCancel.Click += ButtonCancel_Click;
        }

        public override void Close()
        {
            References = new List<string>();

            var items = _dataStore.GetEnumerator();
            while (items.MoveNext())
                References.Add(Tools.Pipeline.PipelineController.Instance.GetRelativePath(items.Current.Location));
            base.Close();
        }

        private void Grid1_SelectionChanged(object sender, EventArgs e)
        {
            buttonRemove.Enabled = grid1.SelectedItems.ToList().Count > 0;
        }

        private void Grid1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Keys.Delete)
                ButtonRemove_Click(sender, e);
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Directory = new Uri(Tools.Pipeline.PipelineController.Instance.ProjectItem.Location);
            dialog.MultiSelect = true;
            dialog.Filters.Add(_dllFileFilter);
            dialog.Filters.Add(_allFileFilter);
            dialog.CurrentFilter = _dllFileFilter;

            if (dialog.ShowDialog(this) == DialogResult.Ok)
                foreach (var fileName in dialog.Filenames)
                    _dataStore.Add(new RefItem(Path.GetFileName(fileName), fileName));
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            var selectedItems = grid1.SelectedItems.ToArray();
            
            foreach (var item in selectedItems)
                _dataStore.Remove(item as RefItem);
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Result = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
