using System.Collections.Generic;
using Eto.Drawing;

namespace MonoGame.Content.Builder.Editor
{
    public interface IController
    {
        IView View { get; }

        Commands Commands { get; }

        Image GetFileIcon(string path, bool link);

        Image GetFolderIcon();

        void UpdateEnabledCommands();

        void LoadProperties(List<IProjectItem> items);
    }
}
