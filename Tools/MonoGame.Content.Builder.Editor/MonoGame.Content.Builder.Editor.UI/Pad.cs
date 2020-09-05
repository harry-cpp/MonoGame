// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor
{
    public abstract class Pad
    {
        private ContextMenu _contextMenu;
        private List<Command> _commands;

        public Pad()
        {
            _contextMenu = new ContextMenu();
            _commands = new List<Command>();
        }

        public abstract Control Control { get; }

        public abstract string Title { get; }

        public void ShowMenu(Control control)
        {
            _contextMenu.Show(control);
        }

        public void AddViewItem(Command command)
        {
            _commands.Add(command);
            _contextMenu.Items.Add(command.CreateMenuItem());
        }

        public abstract void UpdateEnabledCommands(Commands commands);
    }
}
