// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;

namespace MonoGame.Content.Builder.Editor
{
    public interface IView
    {
        void Attach(IController controller, Pad projectPad, Pad propertyPad);

        void UpdateEnabledCommands();

        Bitmap GetFileIcon(string filePath);

        Bitmap GetFolderIcon();

        Bitmap GetLinkIcon();
    }
}
