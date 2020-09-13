// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using MonoGame.Content.Builder.Editor.Project;
using MonoGame.Content.Builder.Editor.Property;

namespace MonoGame.Content.Builder.Editor
{
    public class Controller : IController
    {
        private Commands _commands;
        private PipelineProject _project;
        private readonly FileFilter _mgcbFileFilter, _allFileFilter, _xnaFileFilter;
        private ProjectPad _projectPad;
        private PropertyPad _propertyPad;
        private bool _isProjectDirty;
        private Dictionary<string, Bitmap> _files;
        private Bitmap _folder;
        private Bitmap _link;

        public Controller(IView view)
        {
            _commands = new Commands();
            _mgcbFileFilter = new FileFilter("MonoGame Content Build Project (*.mgcb)", new[] { ".mgcb" });
            _allFileFilter = new FileFilter("All Files (*.*)", new[] { ".*" });
            _xnaFileFilter = new FileFilter("XNA Content Projects (*.contentproj)", new[] { ".contentproj" });
            _files = new Dictionary<string, Bitmap>();
            View = view;

            _commands.NewProject.Executed += (o, e) => NewProject();
            _commands.OpenProject.Executed += (o, e) => OpenProject();
            _commands.ImportProject.Executed += (o, e) => ImportProject();
            _commands.SaveProject.Executed += (o, e) => SaveProject(false);
            _commands.SaveAsProject.Executed += (o, e) => SaveProject(true);
            _commands.CloseProject.Executed += (o, e) => CloseProject();

            _projectPad = new ProjectPad(this);
            _propertyPad = new PropertyPad();

            view.Attach(this, _projectPad, _propertyPad);

            _folder = view.GetFolderIcon();
            _link = view.GetLinkIcon();

            UpdateEnabledCommands();
        }

        public string ProjectLocation
        {
            get
            {
                var ret = (Project == null) ? string.Empty : Project.Location;
                if (!ret.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    ret += Path.DirectorySeparatorChar;

                return ret;
            }
        }

        public bool IsProjectOpen => Project != null;

        private PipelineProject Project
        {
            get => _project;
            set
            {
                _project = value;
                UpdateEnabledCommands();
            }
        }

        public bool IsProjectDirty
        {
            get => _isProjectDirty;
            set
            {
                _isProjectDirty = value;
                UpdateEnabledCommands();
            }
        }

        public IView View { get; private set; }

        public Commands Commands => _commands;

        public void OnProjectModified()
        {
            Debug.Assert(IsProjectOpen, "OnProjectModified called with no project open?");
            IsProjectDirty = true;
        }

        public void OnReferencesModified()
        {
            Debug.Assert(IsProjectOpen, "OnReferencesModified called with no project open?");
            IsProjectDirty = true;
            ResolveTypes();
        }

        public void NewProject()
        {
            // Make sure we give the user a chance to
            // save the project if they need too.
            if (!AskSaveProject())
                return;

            // A project needs a root directory or it is impossible to resolve relative paths.
            // So we need the user to choose that location even though the project has not
            // yet actually been saved to disk.

            var dialog = new SaveFileDialog();
            dialog.Title = "New Project";
            dialog.Filters.Add(_mgcbFileFilter);
            dialog.Filters.Add(_allFileFilter);
            dialog.CurrentFilter = _mgcbFileFilter;

            if (dialog.Show() != DialogResult.Ok)
                return;

            var projectFilePath = dialog.FileName;
            if (dialog.CurrentFilter == _mgcbFileFilter && !projectFilePath.EndsWith(".mgcb"))
                projectFilePath += ".mgcb";

            CloseProject();

            // Clear existing project data, initialize to a new blank project.
            Project = new PipelineProject();
            PipelineTypes.Load(Project);

            // Save the new project.
            Project.OriginalPath = projectFilePath;
            IsProjectDirty = true;
        }

        public void ImportProject()
        {
            // Make sure we give the user a chance to
            // save the project if they need too.
            if (!AskSaveProject())
                return;

            var dialog = new OpenFileDialog();
            dialog.Filters.Add(_xnaFileFilter);
            dialog.Filters.Add(_allFileFilter);
            dialog.CurrentFilter = _xnaFileFilter;

            if (dialog.Show() != DialogResult.Ok)
                return;

            CloseProject();

#if !DEBUG
            try
#endif
            {
                Project = new PipelineProject();
                var parser = new PipelineProjectParser(Project);
                parser.ImportProject(dialog.FileName);

                ResolveTypes();

                IsProjectDirty = true;
            }
#if !DEBUG
            catch (Exception e)
            {
                View.ShowError("Open Project", "Failed to open project!");
                return;
            }
#endif
        }

        public void OpenProject()
        {
            // Make sure we give the user a chance to
            // save the project if they need too.
            if (!AskSaveProject())
                return;

            var dialog = new OpenFileDialog();
            dialog.Title = "Open Project";
            dialog.Filters.Add(_mgcbFileFilter);
            dialog.Filters.Add(_allFileFilter);
            dialog.CurrentFilter = _mgcbFileFilter;

            if (dialog.Show() == DialogResult.Ok)
                OpenProject(dialog.FileName);
        }

        public void OpenProject(string projectFilePath)
        {
            projectFilePath = Path.GetFullPath(projectFilePath);
            CloseProject();

            var errortext = "Failed to open the project due to an unknown error.";

#if !DEBUG
            try
            {
#endif
                Project = new PipelineProject();

                var parser = new PipelineProjectParser(Project);
                var errorCallback = new MGBuildParser.ErrorCallback((msg, args) =>
                {
                    errortext = string.Format(msg, args);
                    throw new Exception();
                });
                parser.OpenProject(projectFilePath, errorCallback);

                ResolveTypes();

                IsProjectDirty = false;

                PipelineSettings.Default.AddProjectHistory(projectFilePath);
                PipelineSettings.Default.Save();
                View.UpdateEnabledCommands();

                _projectPad.Open(Project);
                LoadProperties(new List<IProjectItem>(new[] { Project }));
#if !DEBUG
            }
            catch (Exception)
            {
                MessageBox.Show(null, Path.GetFileName(projectFilePath) + ": " + errortext, "Error Opening Project", MessageBoxButtons.OK, MessageBoxType.Error);
                return;
            }
#endif
        }

        public void ClearRecentList()
        {
            PipelineSettings.Default.ProjectHistory.Clear();
            PipelineSettings.Default.Save();
            View.UpdateEnabledCommands();
        }

        public void CloseProject()
        {
            if (!IsProjectOpen)
                return;

            // Make sure we give the user a chance to
            // save the project if they need too.
            if (!AskSaveProject())
                return;

            IsProjectDirty = false;
            Project = null;

            LoadProperties(null);
            _projectPad.Open(null);
        }

        public bool MoveProject(string newname)
        {
            if (Project == null)
                return false;

            string opath = Project.OriginalPath;
            string ext = Path.GetExtension(opath);

            PipelineSettings.Default.ProjectHistory.Remove(opath);

            try
            {
                File.Delete(Project.OriginalPath);
            }
            catch
            {
                MessageBox.Show(null, "Could not delete old project file.", "Error", MessageBoxButtons.OK, MessageBoxType.Error);
                return false;
            }

            Project.OriginalPath = Path.GetDirectoryName(opath) + Path.DirectorySeparatorChar + newname + ext;
            if (!SaveProject(false))
            {
                Project.OriginalPath = opath;
                SaveProject(false);
                MessageBox.Show(null, "Could not save the new project file.", "Error", MessageBoxButtons.OK, MessageBoxType.Error);
                return false;
            }
            // View.SetTreeRoot(_project);

            return true;
        }

        public bool SaveProject(bool saveAs)
        {
            if (Project == null)
                return false;

            // Do we need file name?
            if (saveAs || string.IsNullOrEmpty(Project.OriginalPath))
            {
                var dialog = new SaveFileDialog();
                dialog.Title = "Save Project";
                dialog.Filters.Add(_mgcbFileFilter);
                dialog.Filters.Add(_allFileFilter);
                dialog.CurrentFilter = _mgcbFileFilter;

                if (!string.IsNullOrEmpty(Project.OriginalPath))
                    dialog.FileName = Project.OriginalPath;

                if (dialog.Show() != DialogResult.Ok)
                    return false;

                var newFilePath = dialog.FileName;
                if (dialog.CurrentFilter == _mgcbFileFilter && !newFilePath.EndsWith(".mgcb"))
                    newFilePath += ".mgcb";

                Project.OriginalPath = newFilePath;
                // View.SetTreeRoot(_project);
            }

            // Do the save.
            IsProjectDirty = false;
            var parser = new PipelineProjectParser(Project);
            parser.SaveProject();

            // Note: This is where a project loaded via 'new project' or 'import project' 
            //       get recorded into PipelineSettings because up until this point they did not
            //       exist as files on disk.
            PipelineSettings.Default.AddProjectHistory(Project.OriginalPath);
            PipelineSettings.Default.Save();
            View.UpdateEnabledCommands();

            return true;
        }

        /// <summary>
        /// Prompt the user if they wish to save the project.
        /// Save it if yes is chosen.
        /// Return true if yes or no is chosen.
        /// Return false if cancel is chosen.
        /// </summary>
        private bool AskSaveProject()
        {
            // If the project is not dirty or open
            // then we can simply skip it.
            if (!IsProjectDirty)
                return true;

            // Ask the user if they want to save or cancel.
            var result = MessageBox.Show(null, "Do you want to save the project first?", "Save Project", MessageBoxButtons.YesNoCancel, MessageBoxType.Question);

            if (result == DialogResult.Cancel)
                return false;

            if (result == DialogResult.No)
                return true;

            return SaveProject(false);
        }

        private void ResolveTypes()
        {
            if (Project == null)
                return;

            PipelineTypes.Load(Project);
            foreach (var i in Project.ContentItems)
                i.ResolveTypes();
        }

        public void LoadProperties(List<IProjectItem> items)
        {
            _propertyPad.Objects = items.Cast<object>().ToList();
        }

        public Image GetFileIcon(string filePath, bool link)
        {
            var key = (link ? '1' : '0') + Path.GetExtension(filePath);
            if (_files.ContainsKey(key))
                return _files[key];

            var icon = View.GetFileIcon(filePath);
            if (link)
            {
                var g = new Graphics(icon);
                g.DrawImage(_link, Point.Empty);
                g.Flush();
            }

            if (File.Exists(filePath))
                _files.Add(key, icon);

            return icon;
        }

        public Image GetFolderIcon()
        {
            return _folder;
        }

        public void UpdateEnabledCommands()
        {
            _commands.NewProject.Enabled = true;
            _commands.OpenProject.Enabled = true;
            _commands.ImportProject.Enabled = true;
            _commands.SaveProject.Enabled = IsProjectOpen && IsProjectDirty;
            _commands.SaveAsProject.Enabled = IsProjectOpen;
            _commands.CloseProject.Enabled = IsProjectOpen;

            _projectPad.UpdateEnabledCommands(_commands);
            _propertyPad.UpdateEnabledCommands(_commands);

            View.UpdateEnabledCommands();
        }
    }
}
