using System;
using Xwt;

namespace MonoGame.Tools.Pipeline
{
    public partial class TextEditDialog: Dialog
    {
        event EventHandler<EventArgs> OkEnabled;

        public string Text;
        bool strict;

        public TextEditDialog(string title, string label, string text, bool strictmode)
        {
            Build();

            this.Title = title;
            label1.Text = label;
            entry1.Text = text;

            strict = strictmode;
            dbOk.Sensitive = !strict;

            entry1.Changed += Entry1_Changed;
        }

        void Entry1_Changed (object sender, EventArgs e)
        {
            if (strict)
            {
                if (Global.CheckString(entry1.Text, Global.NotAllowedCharacters))
                {
                    if (entry1.Text != "")
                    {
                        Text = entry1.Text;
                        label2.Visible = false;
                        dbOk.Sensitive = true;
                        this.Height = 100;
                    }
                    else
                    {
                        label2.Visible = false;
                        dbOk.Sensitive = false;
                    }
                }
                else
                {
                    var chars = Global.NotAllowedCharacters.ToCharArray();
                    string notallowedchars = chars[0].ToString();

                    for (int i = 1; i < chars.Length; i++)
                        notallowedchars += ", " + chars[i];

                    label2.Text = "Your name contains one of not allowed characters: " + notallowedchars;

                    label2.Visible = true;
                    dbOk.Sensitive = false;
                }

                if(OkEnabled != null)
                    OkEnabled(this, new EventArgs());
            }
        }
    }
}

