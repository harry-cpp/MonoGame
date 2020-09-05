using Eto.Forms;

namespace MonoGame.Content.Builder.Editor
{
    public class Commands
    {
        public Command NewProject { get; set; }

        public Command OpenProject { get; set; }

        public Command ImportProject { get; set; }

        public Command SaveProject { get; set; }
        
        public Command SaveAsProject { get; set; }

        public Command CloseProject { get; set; }

        public Commands()
        {
            NewProject = new Command();
            NewProject.MenuText = "New...";

            OpenProject = new Command();
            OpenProject.MenuText = "Open...";

            ImportProject = new Command();
            ImportProject.MenuText = "Import";

            SaveProject = new Command();
            SaveProject.MenuText = "Save...";
            SaveProject.ToolTip = "Save";

            SaveAsProject = new Command();
            SaveAsProject.MenuText = "Save As";

            CloseProject = new Command();
            CloseProject.MenuText = "Close";
        }
    }
}
