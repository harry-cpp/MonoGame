using Xwt;

namespace MonoGame.Tools.Pipeline
{
    public partial class AddItemDialog: Dialog
    {
        public bool ApplyForAll;
        public CopyAction Action;

        public AddItemDialog(FileType type, string fileloc, bool exists)
        {
            Build(type, fileloc);

            if (exists)
            {
                radioCopy.Sensitive = false;
                radioLink.Active = true;
            }
        }

        protected override void OnCommandActivated(Command cmd)
        {
            if (radioCopy.Active)
                Action = CopyAction.Copy;
            else if (radioLink.Active)
                Action = CopyAction.Link;
            else
                Action = CopyAction.Skip;
            ApplyForAll = check1.Active;

            base.OnCommandActivated(cmd);
        }
    }
}

