// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor
{
    public abstract class Pad
    {
        private List<Command> _commands;
        private ContextMenu _contextMenu;

        public Pad()
        {
            _commands = new List<Command>();
            _contextMenu = new ContextMenu();
        }

        public List<Command> ViewCommands => _commands;

        public ContextMenu ContextMenu => _contextMenu;

        public abstract Control Control { get; }

        public abstract string Title { get; }

        public void AddViewItem(Command command)
        {
            _commands.Add(command);
            _contextMenu.Items.Add(command);
        }

        public void AddViewSeparator()
        {
            _commands.Add(null);
            _contextMenu.Items.Add(new SeparatorMenuItem());
        }

        public abstract void UpdateEnabledCommands(Commands commands);
    }
}
