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
            /*
            NewProject.Executed += (o, e) => controller.NewProject();
            OpenProject.Executed += (o, e) => controller.OpenProject();
            ImportProject.Executed += (o, e) => controller.ImportProject();
            SaveProject.Executed += (o, e) => controller.SaveProject(false);
            SaveAsProject.Executed += (o, e) => controller.SaveProject(true);
            CloseProject.Executed += (o, e) => controller.CloseProject();*/
        }

        /*public static void Update()
        {
            NewProject.Enabled = true;
            OpenProject.Enabled = true;
            ImportProject.Enabled = true;
            SaveProject.Enabled = Controller.IsProjectOpen && Controller.IsProjectDirty;
            SaveAsProject.Enabled = Controller.IsProjectOpen;
            CloseProject.Enabled = Controller.IsProjectOpen;
        }*/
    }
}
