﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.ComponentModel;
using System.Linq;

namespace MonoGame.Tools.Pipeline
{
    public class FolderItem : IProjectItem
    {                
        public FolderItem(string path)
        {
            Location = path;
            Name = path;
            if (Name.Contains("/"))
                Name = Name.Split('/').Last();
        }

        [Browsable(false)]
        public string OriginalPath { get { return Location; } }

        [Category("Common")]
        [Description("The name of this folder.")]
        public string Name { get; private set; }

        [Category("Common")]
        [Description("The file path to this folder.")]
        public string Location { get; private set; }

        [Browsable(false)]
        public string Icon { get; set; }

        public bool Exists { get; set; }
#if WINDOWS
        public System.Windows.Forms.TreeNode Node
        {
            get; set; 
        }
#endif
    }
}
