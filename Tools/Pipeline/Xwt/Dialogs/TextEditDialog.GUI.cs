using Xwt;

namespace MonoGame.Tools.Pipeline
{
    public partial class TextEditDialog
    {
        VBox vbox1;
        Label label1, label2;
        TextEntry entry1;
        DialogButton dbOk, dbCancel;

        private void Build()
        {
            this.Width = 400;
            this.Height = 100;

            vbox1 = new VBox();

            label1 = new Label("");
            vbox1.PackStart(label1);

            entry1 = new TextEntry();
            vbox1.PackStart(entry1);

            label2 = new Label("");
            label2.TextAlignment = Alignment.Center;
            label2.Wrap = WrapMode.Word;
            label2.Visible = false;
            label2.Opacity = 0.7;
            vbox1.PackStart(label2);

            this.Content = vbox1;

            dbOk = new DialogButton(Command.Ok);
            dbOk.Sensitive = false;
            this.Buttons.Add(dbOk);

            dbCancel = new DialogButton(Command.Cancel);
            this.Buttons.Add(dbCancel);
        }
    }
}

