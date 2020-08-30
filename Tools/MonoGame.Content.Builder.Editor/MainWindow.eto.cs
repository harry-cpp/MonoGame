// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto;
using Eto.Forms;
using Eto.Drawing;
using MonoGame.Content.Builder.Editor.Project;
using MonoGame.Content.Builder.Editor.Property;

namespace MonoGame.Tools.Pipeline
{
    partial class MainWindow
    {
        /// <summary>
        /// Pipeline menu bar.
        /// Required to Stop Eto Forms adding System Menu Items on MacOS
        /// This is because `IncludeSystemItems` defaults to `All` 
        /// and the menus are populated in the constructor.
        /// </summary>
        class PipelineMenuBar : MenuBar
        {
            public PipelineMenuBar()
            {
                Style = "MenuBar";
                IncludeSystemItems = MenuBarSystemItems.None;
            }
        }

        public Command cmdNew, cmdOpen, cmdClose, cmdImport, cmdSave, cmdSaveAs, cmdExit;
        public static MenuBar MainMenu;

        ButtonMenuItem menuFile, menuRecent;

        ProjectPad projectExplorer;
        PropertyPad propertyGridControl;

        Splitter splitterHorizontal, splitterVertical;

        private void InitializeComponent()
        {
            Title = "MGCB Editor";
            Icon = Icon.FromResource("Icons.monogame.png");
            Size = new Size(900, 600);
            MinimumSize = new Size(400, 400);

            InitalizeCommands();
            InitalizeMenu();

            splitterHorizontal = new Splitter();
            splitterHorizontal.Orientation = Orientation.Horizontal;
            splitterHorizontal.Position = 200;
            splitterHorizontal.Panel1MinimumSize = 100;
            splitterHorizontal.Panel2MinimumSize = 100;

            splitterVertical = new Splitter();
            splitterVertical.Orientation = Orientation.Vertical;
            splitterVertical.Position = 230;
            splitterVertical.FixedPanel = SplitterFixedPanel.None;
            splitterVertical.Panel1MinimumSize = 100;
            splitterVertical.Panel2MinimumSize = 100;

            projectExplorer = new ProjectPad();
            splitterVertical.Panel1 = projectExplorer;

            propertyGridControl = new PropertyPad();
            splitterVertical.Panel2 = propertyGridControl;

            splitterHorizontal.Panel1 = splitterVertical;

            splitterHorizontal.Panel2 = new TextArea();

            Content = splitterHorizontal;

            cmdNew.Executed += CmdNew_Executed;
            cmdOpen.Executed += CmdOpen_Executed;
            cmdClose.Executed += CmdClose_Executed;
            cmdImport.Executed += CmdImport_Executed;
            cmdSave.Executed += CmdSave_Executed;
            cmdSaveAs.Executed += CmdSaveAs_Executed;
            cmdExit.Executed += CmdExit_Executed;
        }

        private void InitalizeCommands()
        {
            // File Commands

            cmdNew = new Command();
            cmdNew.MenuText = "New...";
            cmdNew.ToolTip = "New";
            cmdNew.Image = Global.GetEtoIcon("Commands.New.png");
            cmdNew.Shortcut = Application.Instance.CommonModifier | Keys.N;

            cmdOpen = new Command();
            cmdOpen.MenuText = "Open...";
            cmdOpen.ToolTip = "Open";
            cmdOpen.Image = Global.GetEtoIcon("Commands.Open.png");
            cmdOpen.Shortcut = Application.Instance.CommonModifier | Keys.O;

            cmdClose = new Command();
            cmdClose.MenuText = "Close";
            cmdClose.Image = Global.GetEtoIcon("Commands.Close.png");

            cmdImport = new Command();
            cmdImport.MenuText = "Import";

            cmdSave = new Command();
            cmdSave.MenuText = "Save...";
            cmdSave.ToolTip = "Save";
            cmdSave.Image = Global.GetEtoIcon("Commands.Save.png");
            cmdSave.Shortcut = Application.Instance.CommonModifier | Keys.S;

            cmdSaveAs = new Command();
            cmdSaveAs.MenuText = "Save As";
            cmdSaveAs.Image = Global.GetEtoIcon("Commands.SaveAs.png");

            cmdExit = new Command();
            cmdExit.MenuText = Global.Unix ? "Quit" : "Exit";
            cmdExit.Shortcut = Application.Instance.CommonModifier | Keys.Q;
        }

        private void InitalizeMenu()
        {
            MainMenu = Menu = new PipelineMenuBar();

            menuFile = new ButtonMenuItem();
            menuFile.Text = "&File";
            menuFile.Items.Add(cmdNew);
            menuFile.Items.Add(cmdOpen);

            menuRecent = new ButtonMenuItem();
            menuRecent.Text = "Open Recent";
            menuFile.Items.Add(menuRecent);

            menuFile.Items.Add(cmdClose);
            menuFile.Items.Add(new SeparatorMenuItem());
            menuFile.Items.Add(cmdImport);
            menuFile.Items.Add(new SeparatorMenuItem());
            menuFile.Items.Add(cmdSave);
            menuFile.Items.Add(cmdSaveAs);
            Menu.Items.Add(menuFile);

            Menu.QuitItem = cmdExit.CreateMenuItem();
        }
    }
}

