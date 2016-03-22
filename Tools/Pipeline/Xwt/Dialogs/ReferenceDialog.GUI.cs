using System;
using Xwt;

namespace MonoGame.Tools.Pipeline
{
    public partial class ReferenceDialog
    {
        HBox hbox1;
        VBox vbox1;
        ListView listView1;
        Button btnAdd, btnRemove;
        DialogButton dbOk, dbCancel;

        private void Build()
        {
            this.Title = "Reference Editor";
            this.Width = 320;
            this.Height = 350;

            hbox1 = new HBox();

            listView1 = new ListView();
            hbox1.PackStart(listView1, true);

            vbox1 = new VBox();

            btnAdd = new Button("Add");
            vbox1.PackStart(btnAdd);

            btnRemove = new Button("Remove");
            btnRemove.Sensitive = false;
            vbox1.PackStart(btnRemove);

            hbox1.PackStart(vbox1);

            this.Content = hbox1;

            dbOk = new DialogButton(Command.Ok);
            this.Buttons.Add(dbOk);

            dbCancel = new DialogButton(Command.Cancel);
            this.Buttons.Add(dbCancel);

            btnAdd.Clicked += BtnAdd_Clicked;
            btnRemove.Clicked += BtnRemove_Clicked;
        }
    }
}

