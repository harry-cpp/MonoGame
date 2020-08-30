// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor
{
    public static partial class Controller
    {
        private static PipelineProject? _project;
        private static readonly FileFilter _mgcbFileFilter, _allFileFilter, _xnaFileFilter;

        static Controller()
        {
            _mgcbFileFilter = new FileFilter("MonoGame Content Build Project (*.mgcb)", new[] { ".mgcb" });
            _allFileFilter = new FileFilter("All Files (*.*)", new[] { ".*" });
            _xnaFileFilter = new FileFilter("XNA Content Projects (*.contentproj)", new[] { ".contentproj" });

            View = null!;
        }

        public static void Init(IView view)
        {
            View = view;

            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (Directory.Exists(Path.Combine(root, "..", "Resources", "Templates")))
            {
                root = Path.Combine(root, "..", "Resources");
            }

            view.UpdateRecentList(PipelineSettings.Default.ProjectHistory);
            view.Attach();
        }

        public static PipelineProject? ProjectItem => _project;

        public static string ProjectLocation
        {
            get
            {
                var ret = (_project == null) ? string.Empty : _project.Location;
                if (!ret.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    ret += Path.DirectorySeparatorChar;

                return ret;
            }
        }

        public static bool IsProjectOpen => _project != null;

        public static bool IsProjectDirty { get; set; }

        public static IView View { get; private set; }

        public static void OnProjectModified()
        {
            Debug.Assert(IsProjectOpen, "OnProjectModified called with no project open?");
            IsProjectDirty = true;
        }

        public static void OnReferencesModified()
        {
            Debug.Assert(IsProjectOpen, "OnReferencesModified called with no project open?");
            IsProjectDirty = true;
            ResolveTypes();
        }

        public static void NewProject()
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
            _project = new PipelineProject();
            PipelineTypes.Load(_project);

            // Save the new project.
            _project.OriginalPath = projectFilePath;
            IsProjectDirty = true;
        }

        public static void ImportProject()
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
                _project = new PipelineProject();
                var parser = new PipelineProjectParser(_project);
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

        public static void OpenProject()
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

        public static void OpenProject(string projectFilePath)
        {
            projectFilePath = Path.GetFullPath(projectFilePath);
            CloseProject();

            var errortext = "Failed to open the project due to an unknown error.";

            try
            {
                _project = new PipelineProject();

                var parser = new PipelineProjectParser(_project);
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
                View.UpdateRecentList(PipelineSettings.Default.ProjectHistory);

                View.ProjectPad.Open(_project);
            }
            catch (Exception)
            {
                MessageBox.Show(null, Path.GetFileName(projectFilePath) + ": " + errortext, "Error Opening Project", MessageBoxButtons.OK, MessageBoxType.Error);
                return;
            }
        }

        public static void ClearRecentList()
        {
            PipelineSettings.Default.ProjectHistory.Clear();
            PipelineSettings.Default.Save();
            View.UpdateRecentList(PipelineSettings.Default.ProjectHistory);
        }

        public static void CloseProject()
        {
            if (!IsProjectOpen)
                return;

            // Make sure we give the user a chance to
            // save the project if they need too.
            if (!AskSaveProject())
                return;

            IsProjectDirty = false;
            _project = null;

            View.ProjectPad.Open(_project);
        }

        public static bool MoveProject(string newname)
        {
            if (_project == null)
                return false;

            string opath = _project.OriginalPath;
            string ext = Path.GetExtension(opath);

            PipelineSettings.Default.ProjectHistory.Remove(opath);

            try
            {
                File.Delete(_project.OriginalPath);
            }
            catch
            {
                MessageBox.Show(null, "Could not delete old project file.", "Error", MessageBoxButtons.OK, MessageBoxType.Error);
                return false;
            }

            _project.OriginalPath = Path.GetDirectoryName(opath) + Path.DirectorySeparatorChar + newname + ext;
            if (!SaveProject(false))
            {
                _project.OriginalPath = opath;
                SaveProject(false);
                MessageBox.Show(null, "Could not save the new project file.", "Error", MessageBoxButtons.OK, MessageBoxType.Error);
                return false;
            }
            // View.SetTreeRoot(_project);

            return true;
        }

        public static bool SaveProject(bool saveAs)
        {
            if (_project == null)
                return false;

            // Do we need file name?
            if (saveAs || string.IsNullOrEmpty(_project.OriginalPath))
            {
                var dialog = new SaveFileDialog();
                dialog.Title = "Save Project";
                dialog.Filters.Add(_mgcbFileFilter);
                dialog.Filters.Add(_allFileFilter);
                dialog.CurrentFilter = _mgcbFileFilter;

                if (!string.IsNullOrEmpty(_project.OriginalPath))
                    dialog.FileName = _project.OriginalPath;

                if (dialog.Show() != DialogResult.Ok)
                    return false;

                var newFilePath = dialog.FileName;
                if (dialog.CurrentFilter == _mgcbFileFilter && !newFilePath.EndsWith(".mgcb"))
                    newFilePath += ".mgcb";

                _project.OriginalPath = newFilePath;
                // View.SetTreeRoot(_project);
            }

            // Do the save.
            IsProjectDirty = false;
            var parser = new PipelineProjectParser(_project);
            parser.SaveProject();

            // Note: This is where a project loaded via 'new project' or 'import project' 
            //       get recorded into PipelineSettings because up until this point they did not
            //       exist as files on disk.
            PipelineSettings.Default.AddProjectHistory(_project.OriginalPath);
            PipelineSettings.Default.Save();
            View.UpdateRecentList(PipelineSettings.Default.ProjectHistory);

            return true;
        }

        /// <summary>
        /// Prompt the user if they wish to save the project.
        /// Save it if yes is chosen.
        /// Return true if yes or no is chosen.
        /// Return false if cancel is chosen.
        /// </summary>
        private static bool AskSaveProject()
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

        private static void ResolveTypes()
        {
            if (_project == null)
                return;

            PipelineTypes.Load(_project);
            foreach (var i in _project.ContentItems)
                i.ResolveTypes();
        }

        public static string GetFullPath(string filePath)
        {
            if (_project == null || Path.IsPathRooted(filePath))
            {
                if (filePath.Length == 2 && filePath[0] != '/')
                    filePath += "\\";
                return filePath;
            }

            filePath = filePath.Replace("/", Path.DirectorySeparatorChar.ToString());
            if (filePath.StartsWith("\\"))
                filePath = filePath.Substring(1);

            return _project.Location + Path.DirectorySeparatorChar + filePath;
        }

        public static string GetRelativePath(string path)
        {
            if (!IsProjectOpen)
                return path;

            var dirUri = new Uri(ProjectLocation);
            var fileUri = new Uri(path);
            var relativeUri = dirUri.MakeRelativeUri(fileUri);

            if (relativeUri == null)
                return path;

            return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static void LoadProperties(List<IProjectItem> items)
        {
            View.PropertyPad.SetObjects(items);
        }

        public static Image GetFileIcon(string path, bool link)
        {
            return View.GetFileIcon(path, link);
        }

        public static Image GetFolderIcon()
        {
            return View.GetFolderIcon();
        }

        public static Image GetImageForResource(string resourceId)
        {
            return View.GetImageForResource(resourceId);
        }
    }
}
