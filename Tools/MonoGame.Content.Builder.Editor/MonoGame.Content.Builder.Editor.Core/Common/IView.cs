// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Drawing;
using MonoGame.Content.Builder.Editor.Project;
using MonoGame.Content.Builder.Editor.Property;

namespace MonoGame.Content.Builder.Editor
{
    public interface IView
    {
        void Attach(ProjectPad projectPad, PropertyPad propertyPad);

        void UpdateRecentList(List<string> recentList);

        Image GetFileIcon(string path, bool link);

        Image GetFolderIcon();

        Image? GetImageForResource(string filePath);
    }
}
