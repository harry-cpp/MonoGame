// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Eto.Forms;
using Eto.Drawing;
using System.Reflection;

namespace MonoGame.Tools.Pipeline
{
    partial class MainWindow : Form, IView
    {
#pragma warning disable 649
        public EventHandler<EventArgs> RecentChanged;
        public EventHandler<EventArgs> TitleChanged;
#pragma warning restore 649
        public const string TitleBase = "MGCB Editor";
        public static MainWindow Instance;

        private Clipboard _clipboard;
        private ContextMenu _contextMenu;
        private FileFilter _mgcbFileFilter, _allFileFilter, _xnaFileFilter;

        public MainWindow()
        {
            _clipboard = new Clipboard();

            InitializeComponent();

            Instance = this;

            _contextMenu = new ContextMenu();

            _mgcbFileFilter = new FileFilter("MonoGame Content Build Project (*.mgcb)", new[] { ".mgcb" });
            _allFileFilter = new FileFilter("All Files (*.*)", new[] { ".*" });
            _xnaFileFilter = new FileFilter("XNA Content Projects (*.contentproj)", new[] { ".contentproj" });
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !PipelineController.Instance.Exit();

            base.OnClosing(e);
        }

        public void InitSolutionExplorer(PipelineProject project)
        {
            projectExplorer.Open(project);
        }

        #region IView implements

        public void Attach(IController controller)
        {
            Style = "MainWindow";
        }

        public void Invoke(Action action)
        {
            Application.Instance.Invoke(action);
        }

        public AskResult AskSaveOrCancel()
        {
            var result = MessageBox.Show(this, "Do you want to save the project first?", "Save Project", MessageBoxButtons.YesNoCancel, MessageBoxType.Question);

            if (result == DialogResult.Yes)
                return AskResult.Yes;
            if (result == DialogResult.No)
                return AskResult.No;

            return AskResult.Cancel;
        }

        public bool AskSaveName(ref string filePath, string title)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = title;
            dialog.Filters.Add(_mgcbFileFilter);
            dialog.Filters.Add(_allFileFilter);
            dialog.CurrentFilter = _mgcbFileFilter;

            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                filePath = dialog.FileName;
                if (dialog.CurrentFilter == _mgcbFileFilter && !filePath.EndsWith(".mgcb"))
                    filePath += ".mgcb";
                
                return true;
            }

            return false;
        }

        public bool AskOpenProject(out string projectFilePath)
        {
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(_mgcbFileFilter);
            dialog.Filters.Add(_allFileFilter);
            dialog.CurrentFilter = _mgcbFileFilter;

            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                projectFilePath = dialog.FileName;
                return true;
            }

            projectFilePath = "";
            return false;
        }

        public bool AskImportProject(out string projectFilePath)
        {
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(_xnaFileFilter);
            dialog.Filters.Add(_allFileFilter);
            dialog.CurrentFilter = _xnaFileFilter;

            if (dialog.ShowDialog(this) == DialogResult.Ok)
            {
                projectFilePath = dialog.FileName;
                return true;
            }

            projectFilePath = "";
            return false;
        }

        public void ShowError(string title, string message)
        {
            MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxType.Error);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(this, message, "Info", MessageBoxButtons.OK, MessageBoxType.Information);
        }

        public void UpdateRecentList(List<string> recentList)
        {
            if (RecentChanged != null)
            {
                RecentChanged(recentList, EventArgs.Empty);
                return;
            }

            menuRecent.Items.Clear();

            foreach (var recent in recentList)
            {
                var item = new ButtonMenuItem();
                item.Text = recent;
                item.Click += (sender, e) => PipelineController.Instance.OpenProject(recent);

                menuRecent.Items.Insert(0, item);
            }

            if (menuRecent.Items.Count > 0)
            {
                menuRecent.Items.Add(new SeparatorMenuItem());
                var clearItem = new ButtonMenuItem();
                clearItem.Text = "Clear";
                clearItem.Click += (sender, e) => PipelineController.Instance.ClearRecentList();
                menuRecent.Items.Add(clearItem);
            }
        }

        #endregion

        #region Commands

        private void CmdNew_Executed(object sender, EventArgs e)
        {
            PipelineController.Instance.NewProject();
        }

        private void CmdOpen_Executed(object sender, EventArgs e)
        {
            PipelineController.Instance.OpenProject();
        }

        private void CmdClose_Executed(object sender, EventArgs e)
        {
            PipelineController.Instance.CloseProject();
        }

        private void CmdImport_Executed(object sender, EventArgs e)
        {
            PipelineController.Instance.ImportProject();
        }

        private void CmdSave_Executed(object sender, EventArgs e)
        {
            PipelineController.Instance.SaveProject(false);
        }

        private void CmdSaveAs_Executed(object sender, EventArgs e)
        {
            PipelineController.Instance.SaveProject(true);
        }

        private void CmdExit_Executed(object sender, EventArgs e)
        {
            Application.Instance.Quit();
        }

        #endregion

    }
}

