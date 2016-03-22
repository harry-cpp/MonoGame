using Xwt;

namespace MonoGame.Tools.Pipeline
{
    public partial class AddItemDialog
    {
        VBox vbox1, vbox2;
        Label label1;
        DialogButton dbOk, dbCancel;
        RadioButton radioCopy, radioLink, radioSkip;
        CheckBox check1;

        private void Build(FileType filetype, string fileloc)
        {
            this.Title = "Add " + filetype + " Action";
            this.Width = 410;
            this.Height = 100;

            vbox1 = new VBox();
            vbox1.Spacing = 13;

            label1 = new Label("");
            label1.Wrap = WrapMode.Word;
            label1.Markup = "The " + filetype.ToString().ToLower() + " <b>" + fileloc + "</b> is outside of the target directory. What would you like to do?";
            vbox1.PackStart(label1);

            vbox2 = new VBox();
            vbox2.Spacing = 4;

            radioCopy = new RadioButton("Copy the " + filetype.ToString().ToLower() + " to the directory");
            vbox2.PackStart(radioCopy);

            radioLink = new RadioButton("Add a link to the " + filetype.ToString().ToLower());
            radioLink.Group = radioCopy.Group;
            vbox2.PackStart(radioLink);

            radioSkip = new RadioButton("Skip adding the " + filetype.ToString().ToLower());
            radioSkip.Group = radioCopy.Group;
            vbox2.PackStart(radioSkip);

            vbox1.PackStart(vbox2);

            check1 = new CheckBox("Use the same action for all selected " + filetype.ToString().ToLower() + "s");
            vbox1.PackStart(check1);

            this.Content = vbox1;

            dbOk = new DialogButton(Command.Ok);
            this.Buttons.Add(dbOk);

            dbCancel = new DialogButton(Command.Cancel);
            this.Buttons.Add(dbCancel);
        }
    }
}

