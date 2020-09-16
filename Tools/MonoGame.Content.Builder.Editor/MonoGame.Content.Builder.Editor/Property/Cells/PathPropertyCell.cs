// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    class PathPropertyCell : PropertyCell
    {
        public override Control Edit()
        {
            var basePath = string.Empty;
            if (ParentObjects.Count > 0 && ParentObjects[0] is PipelineProject projectItem)
                basePath = projectItem.Location;

            var pathDialog = new PathDialog(basePath);
            pathDialog.DisplayMode = DialogDisplayMode.Attached;
            pathDialog.FolderPath = Value.ToString();

            if (pathDialog.Show() == DialogResult.Ok)
                Value = pathDialog.FolderPath;

            return null;
        }
    }
}
