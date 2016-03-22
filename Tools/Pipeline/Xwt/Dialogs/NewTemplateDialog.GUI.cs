using Xwt;

namespace MonoGame.Tools.Pipeline
{
    public partial class NewTemplateDialog
    {
        VBox vbox1;
        HBox hbox1;
        ListView listView1;
        Label label1, label2;
        TextEntry entry1;
        DialogButton dbOk, dbCancel;

        protected void Build ()
        {
            this.Title = "New Item";
            this.Width = 350;
            this.Height = 365;
            this.Padding = 4;

            vbox1 = new VBox ();
            vbox1.Spacing = 2;

            listView1 = new ListView ();
            listView1.SelectionMode = SelectionMode.Single;
            vbox1.PackStart (listView1, true);

            hbox1 = new HBox ();

            label1 = new Label ("Name: ");
            hbox1.PackStart (label1);

            entry1 = new TextEntry ();
            hbox1.PackStart (entry1, true);

            vbox1.PackStart (hbox1);

            label2 = new Label ("");
            label2.TextAlignment = Alignment.Center;
            label2.Wrap = WrapMode.Word;
            label2.Visible = false;
            label2.Opacity = 0.7;
            vbox1.PackStart (label2);

            Content = vbox1;

            dbOk = new DialogButton (Command.Ok);
            dbOk.Sensitive = false;
            this.Buttons.Add (dbOk);

            dbCancel = new DialogButton (Command.Cancel);
            this.Buttons.Add (dbCancel);
        }
    }
}
