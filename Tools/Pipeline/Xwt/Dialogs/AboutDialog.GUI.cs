using Xwt;
using Xwt.Drawing;

namespace MonoGame.Tools.Pipeline
{
    public partial class AboutDialog
    {
        VBox vbox1;
        ImageView image1;
        Label labelProgramName, labelVersion, labelComments, labelCopyright;
        LinkLabel labelWebsite;

        private void Build()
        {
            this.Title = "About";
            this.Resizable = false;
            this.Buttons.Add(new DialogButton(Command.Close));

            vbox1 = new VBox();

            image1 = new ImageView();
            image1.Image = Image.FromResource("MonoGame.Tools.Pipeline.App.ico");
            vbox1.PackStart(image1);

            labelProgramName = new Label();
            labelProgramName.TextAlignment = Alignment.Center;
            vbox1.PackStart(labelProgramName);

            labelVersion = new Label();
            labelVersion.TextAlignment = Alignment.Center;
            vbox1.PackStart(labelVersion);

            labelComments = new Label();
            labelComments.TextAlignment = Alignment.Center;
            vbox1.PackStart(labelComments);

            labelWebsite = new LinkLabel();
            labelWebsite.TextAlignment = Alignment.Center;
            vbox1.PackStart(labelWebsite);

            labelCopyright = new Label();
            labelCopyright.TextAlignment = Alignment.Center;
            vbox1.PackStart(labelCopyright);

            this.Content = vbox1;
        }
    }
}

