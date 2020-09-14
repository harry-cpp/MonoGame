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
            var pathDialog = new PathDialog();
            pathDialog.FolderPath = Value.ToString();

            if (pathDialog.Show() == DialogResult.Ok)
            {
                Value = pathDialog.FolderPath;
            }

            return null;
        }
    }
}
