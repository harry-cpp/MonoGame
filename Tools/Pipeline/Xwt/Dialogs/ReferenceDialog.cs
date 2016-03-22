using System;
using Xwt;
using System.IO;
using System.Collections.Generic;

namespace MonoGame.Tools.Pipeline
{
    public partial class ReferenceDialog: Dialog
    {
        public List<string> References = new List<string>();

        MainWindow window;
        ListStore _store;
        DataField<string> _textCol;
        DataField<string> _textCol2;

        public ReferenceDialog(Window win, List<string> refs)
        {
            Build();

            window = (MainWindow)win;

            _textCol = new DataField<string>();
            _textCol2 = new DataField<string>();

            _store = new ListStore(_textCol, _textCol2);
            listView1.DataSource = _store;
            listView1.Columns.Add("Name           ", _textCol).CanResize = true;
            listView1.Columns.Add("Location", _textCol2).CanResize = true;
            listView1.SelectionMode = SelectionMode.Multiple;

            listView1.SelectionChanged += ListView1_SelectionChanged;

            foreach (string r in refs)
                AddItem(r);
        }

        private void AddItem(string path)
        {
            int row = _store.AddRow();
            _store.SetValue(row, _textCol, Path.GetFileName(path));
            _store.SetValue(row, _textCol2, window._controller.GetFullPath(path));
            
            References.Add(path);
        }

        protected void ListView1_SelectionChanged (object sender, EventArgs e)
        {
            btnRemove.Sensitive = (listView1.SelectedRows.Length > 0);
        }

        protected void BtnAdd_Clicked (object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;

            dialog.Filters.Add(new FileDialogFilter("Dll Files (*.dll)", "*.dll"));
            dialog.Filters.Add(new FileDialogFilter("All Files (*.*)", "*.*"));

            if (dialog.Run(window))
            {
                string pl = ((PipelineController)window._controller).ProjectLocation;
                if (!pl.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    pl += Path.DirectorySeparatorChar;
                var folderUri = new Uri(pl);

                foreach (string filename in dialog.FileNames)
                {
                    var pathUri = new Uri(filename);
                    var fl = Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', System.IO.Path.DirectorySeparatorChar));

                    AddItem(fl);
                }
            }
        }

        protected void BtnRemove_Clicked (object sender, EventArgs e)
        {
            int[] rows = listView1.SelectedRows;

            for (int i = rows.Length - 1; i >= 0; i--)
            {
                _store.RemoveRow(rows[i]);
                References.RemoveAt(rows[i]);
            }
        }
    }
}

