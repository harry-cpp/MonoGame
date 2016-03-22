using System;
using System.Collections.Generic;
using Xwt;
using Xwt.Drawing;

namespace MonoGame.Tools.Pipeline
{
    public partial class NewTemplateDialog : Dialog
    {
        event EventHandler<EventArgs> OkEnabled;

        public string Name;
        public ContentItemTemplate TemplateFile;

        DataField<Image> _imgCol = new DataField<Image> ();
        DataField<string> _textCol = new DataField<string> ();

        ListStore _listStore;
        List<ContentItemTemplate> items;

        public NewTemplateDialog (IEnumerator<ContentItemTemplate> enums)
        {
            Build ();

            _listStore = new ListStore (_imgCol, _textCol);
            listView1.DataSource = _listStore;

            listView1.Columns.Add ("", _imgCol, _textCol);
            listView1.HeadersVisible = false;

            items = new List<ContentItemTemplate> ();

            while (enums.MoveNext ()) {

                int row = _listStore.AddRow ();
                _listStore.SetValue (row, _imgCol, Image.FromFile (System.IO.Path.GetDirectoryName (enums.Current.TemplateFile) + "/" + enums.Current.Icon));
                _listStore.SetValue (row, _textCol, enums.Current.Label);

                items.Add (enums.Current);
            }

            listView1.SelectionChanged += ListView1_SelectionChanged;
            entry1.Changed += Entry1_Changed;
        }

        void ListView1_SelectionChanged (object sender, EventArgs e)
        {
            buttonOkEnabled ();
        }

        private void Entry1_Changed (object sender, EventArgs e)
        {
            buttonOkEnabled ();
        }

        private void buttonOkEnabled ()
        {
            if (Global.CheckString (entry1.Text, Global.NotAllowedCharacters)) {
                label2.Visible = false;

                if (listView1.SelectedRows.Length > 0 && entry1.Text != "") {
                    TemplateFile = items [listView1.SelectedRow];
                    Name = entry1.Text;

                    dbOk.Sensitive = true;

                    if (OkEnabled != null)
                        OkEnabled (this, new EventArgs ());
                    return;
                }
            } else {
                label2.Visible = true;

                var chars = Global.NotAllowedCharacters.ToCharArray ();
                string notallowedchars = chars [0].ToString ();

                for (int i = 1; i < chars.Length; i++)
                    notallowedchars += ", " + chars [i];

                label2.Text = "Your name contains one of not allowed characters: " + notallowedchars;
            }

            dbOk.Sensitive = false;
            if (OkEnabled != null)
                OkEnabled (this, new EventArgs ());
        }
    }
}
