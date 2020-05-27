// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Content.Builder;
using MonoGame.Tools.Pipeline.Utilities;
using PathHelper = MonoGame.Framework.Content.Pipeline.Builder.PathHelper;

namespace MonoGame.Tools.Pipeline
{
    public partial class PipelineController : IController
    {
        public static PipelineController Instance;

        private PipelineProject _project;

        private Task _buildTask;
        private Process _buildProcess;

        private static readonly string [] _mgcbSearchPaths = new []       
        {
#if DEBUG
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../../../MonoGame.Content.Builder/Debug"),
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../../../../../../MonoGame.Content.Builder/Debug"),
#else
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../../../MonoGame.Content.Builder/Release"),
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "../../../../../../MonoGame.Content.Builder/Release"),
#endif
        };

        public PipelineProject ProjectItem
        {
            get
            {
                return _project;
            }
        }

        public string ProjectLocation
        {
            get
            {
                var ret = _project.Location;

                if (!_project.Location.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    ret += Path.DirectorySeparatorChar;
                
                return ret; 
            }
        }

        public string ProjectOutputDir
        {
            get { return _project.OutputDir; }
        }

        public List<IProjectItem> SelectedItems { get; private set; }

        public IProjectItem SelectedItem { get; private set; }
        
        public bool ProjectOpen { get; private set; }

        public bool ProjectDirty { get; set; }

        public bool ProjectBuilding 
        {
            get
            {
                return _buildTask != null && !_buildTask.IsCompleted;
            }
        }

        public IView View { get; private set; }

        public event Action OnProjectLoading;

        public event Action OnProjectLoaded;

        private PipelineController(IView view)
        {
            Instance = this;

            SelectedItems = new List<IProjectItem>();

            View = view;
            View.Attach(this);
            ProjectOpen = false;

            var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (Directory.Exists(Path.Combine (root, "..", "Resources", "Templates")))
            {
                root = Path.Combine(root, "..", "Resources");
            }

            view.UpdateRecentList(PipelineSettings.Default.ProjectHistory);
        }

        public static PipelineController Create(IView view)
        {
            return new PipelineController(view);
        }

        public void OnProjectModified()
        {            
            Debug.Assert(ProjectOpen, "OnProjectModified called with no project open?");
            ProjectDirty = true;
        }

        public void OnReferencesModified()
        {
            Debug.Assert(ProjectOpen, "OnReferencesModified called with no project open?");
            ProjectDirty = true;
            ResolveTypes();
        }

        public void OnItemModified(ContentItem contentItem)
        {
            Debug.Assert(ProjectOpen, "OnItemModified called with no project open?");
            ProjectDirty = true;

            // View.BeginTreeUpdate();
            // View.UpdateTreeItem(contentItem);
            // View.EndTreeUpdate();
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
            var projectFilePath = Environment.CurrentDirectory;
            if (!View.AskSaveName(ref projectFilePath, "New Project"))
                return;

            CloseProject();

            if (OnProjectLoading != null)
                OnProjectLoading();

            // Clear existing project data, initialize to a new blank project.
            _project = new PipelineProject();            
            PipelineTypes.Load(_project);

            // Save the new project.
            _project.OriginalPath = projectFilePath;
            ProjectOpen = true;
            ProjectDirty = true;

            UpdateTree();

            if (OnProjectLoaded != null)
                OnProjectLoaded();
        }

        public void ImportProject()
        {
            // Make sure we give the user a chance to
            // save the project if they need too.
            if (!AskSaveProject())
                return;

            string projectFilePath;
            if (!View.AskImportProject(out projectFilePath))
                return;

            CloseProject();

            if (OnProjectLoading != null)
                OnProjectLoading();

#if SHIPPING
            try
#endif
            {
                _project = new PipelineProject();
                var parser = new PipelineProjectParser(this, _project);
                parser.ImportProject(projectFilePath);

                ResolveTypes();                
                
                ProjectOpen = true;
                ProjectDirty = true;                
            }
#if SHIPPING
            catch (Exception e)
            {
                View.ShowError("Open Project", "Failed to open project!");
                return;
            }
#endif

            UpdateTree();
            // View.UpdateTreeItem(_project);

            if (OnProjectLoaded != null)
                OnProjectLoaded();
        }

        public void OpenProject()
        {
            // Make sure we give the user a chance to
            // save the project if they need too.
            if (!AskSaveProject())
                return;

            string projectFilePath;
            if (!View.AskOpenProject(out projectFilePath))
                return;
            
            OpenProject(projectFilePath);
        }

        public void OpenProject(string projectFilePath)
        {
            projectFilePath = Path.GetFullPath(projectFilePath);
            CloseProject();

            if (OnProjectLoading != null)
                OnProjectLoading();

            var errortext = "Failed to open the project due to an unknown error.";

            try
            {
                _project = new PipelineProject();
                
                var parser = new PipelineProjectParser(this, _project);
                var errorCallback = new MGBuildParser.ErrorCallback((msg, args) =>
                {
                    errortext = string.Format(msg, args);
                    throw new Exception();
                });
                parser.OpenProject(projectFilePath, errorCallback);

                ResolveTypes();

                ProjectOpen = true;
                ProjectDirty = false;

                PipelineSettings.Default.AddProjectHistory(projectFilePath);
                PipelineSettings.Default.StartupProject = projectFilePath;
                PipelineSettings.Default.Save();
                View.UpdateRecentList(PipelineSettings.Default.ProjectHistory);

                View.InitSolutionExplorer(_project);
            }
            catch (Exception)
            {
                View.ShowError("Error Opening Project", Path.GetFileName(projectFilePath) + ": " + errortext);
                return;
            }

            UpdateTree();

            if (OnProjectLoaded != null)
                OnProjectLoaded();
        }

        public void ClearRecentList()
        {
            PipelineSettings.Default.ProjectHistory.Clear();
            PipelineSettings.Default.Save();
            View.UpdateRecentList(PipelineSettings.Default.ProjectHistory);
        }

        public void CloseProject()
        {
            if (!ProjectOpen)
                return;

            // Make sure we give the user a chance to
            // save the project if they need too.
            if (!AskSaveProject())
                return;

            ProjectOpen = false;
            ProjectDirty = false;
            _project = null;

            PipelineSettings.Default.StartupProject = null;
            PipelineSettings.Default.Save();

            UpdateTree();
        }

        public bool MoveProject(string newname)
        {
            string opath = _project.OriginalPath;
            string ext = Path.GetExtension(opath);

            PipelineSettings.Default.ProjectHistory.Remove(opath);

            try
            {
                File.Delete(_project.OriginalPath);
            }
            catch {
                View.ShowError("Error", "Could not delete old project file.");
                return false;
            }

            _project.OriginalPath = Path.GetDirectoryName(opath) + Path.DirectorySeparatorChar + newname + ext;
            if (!SaveProject(false))
            {
                _project.OriginalPath = opath;
                SaveProject(false);
                View.ShowError("Error", "Could not save the new project file.");
                return false;
            }
            // View.SetTreeRoot(_project);

            return true;
        }
        
        public bool SaveProject(bool saveAs)
        {
            // Do we need file name?
            if (saveAs || string.IsNullOrEmpty(_project.OriginalPath))
            {
                string newFilePath = _project.OriginalPath;
                if (!View.AskSaveName(ref newFilePath, null))
                    return false;

                _project.OriginalPath = newFilePath;
				// View.SetTreeRoot(_project);
            }

            // Do the save.
            ProjectDirty = false;
            var parser = new PipelineProjectParser(this, _project);
            parser.SaveProject();

            // Note: This is where a project loaded via 'new project' or 'import project' 
            //       get recorded into PipelineSettings because up until this point they did not
            //       exist as files on disk.
            PipelineSettings.Default.AddProjectHistory(_project.OriginalPath);
            PipelineSettings.Default.StartupProject = _project.OriginalPath;
            PipelineSettings.Default.Save();
            View.UpdateRecentList(PipelineSettings.Default.ProjectHistory);

            return true;
        }

        public void Build(bool rebuild)
        {
            var commands = string.Format("/@:\"{0}\" {1}", _project.OriginalPath, rebuild ? "/rebuild" : string.Empty);
            if (PipelineSettings.Default.DebugMode)
                commands += " /launchdebugger";
            BuildCommand(commands);
        }

        private IEnumerable<IProjectItem> GetItems(IProjectItem dir)
        {
            foreach (var item in _project.ContentItems)
                if (item.OriginalPath.StartsWith(dir.OriginalPath + "/"))
                    yield return item;
        }

        public void RebuildItems()
        {
            var items = new List<IProjectItem>();

            // If the project itself was selected, just
            // rebuild the entire project
            if (items.Contains(_project))
            {
                Build(true);
                return;
            }

            // Convert selected DirectoryItems into ContentItems
            foreach (var item in SelectedItems)
            {
                if (item is ContentItem)
                {
                    if (!items.Contains(item))
                        items.Add(item);
                    
                    continue;
                }

                foreach (var subitem in GetItems(item))
                    if (!items.Contains(subitem))
                        items.Add(subitem);
            }

            // Create a unique file within the same folder as
            // the normal project to store this incremental build.
            var uniqueName = Guid.NewGuid().ToString();
            var tempPath = Path.Combine(Path.GetDirectoryName(_project.OriginalPath), uniqueName);

            // Write the incremental project file limiting the
            // content to just the files we want to rebuild.
            using (var io = File.CreateText(tempPath))
            {
                var parser = new PipelineProjectParser(this, _project);
                parser.SaveProject(io, (i) => !items.Contains(i));
            }

            // Run the build the command.
            var commands = string.Format("/@:\"{0}\" /rebuild /incremental", tempPath);
            if (PipelineSettings.Default.DebugMode)
                commands += " /launchdebugger";
            BuildCommand(commands);

            // Cleanup the temp file once we're done.
            _buildTask.ContinueWith((e) => File.Delete(tempPath));
        }

        private void BuildCommand(string commands)
        {
            Debug.Assert(_buildTask == null || _buildTask.IsCompleted, "The previous build wasn't completed!");

            if (ProjectDirty)
                SaveProject(false);

            _buildTask = Task.Factory.StartNew(() => DoBuild(commands));
        }

        public void Clean()
        {
            Debug.Assert(_buildTask == null || _buildTask.IsCompleted, "The previous build wasn't completed!");

            // Make sure we save first!
            if (!AskSaveProject())
                return;

            var commands = string.Format("/clean /intermediateDir:\"{0}\" /outputDir:\"{1}\"", _project.IntermediateDir, _project.OutputDir);
            if (PipelineSettings.Default.DebugMode)
                commands += " /launchdebugger";

            _buildTask = Task.Factory.StartNew(() => DoBuild(commands));
        }

        private void DoBuild(string commands)
        {
            Encoding encoding;
            try {
                encoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            } catch (NotSupportedException) {
                encoding = Encoding.UTF8;
            } catch (ArgumentException) {
                encoding = Encoding.UTF8;
            }
            
            var currentDir = Environment.CurrentDirectory;
            try
            {
                // Prepare the process.
                _buildProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "mgcb",
                        Arguments = commands,
                        WorkingDirectory = Path.GetDirectoryName(_project.OriginalPath),
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        StandardOutputEncoding = encoding
                    }.ResolveDotnetApp(_mgcbSearchPaths, waitForExit: true)
                };

                // Fire off the process.
                Console.WriteLine(_buildProcess.StartInfo.FileName + " " + _buildProcess.StartInfo.Arguments);
                Environment.CurrentDirectory = _buildProcess.StartInfo.WorkingDirectory;
                _buildProcess.Start();
                _buildProcess.BeginOutputReadLine();
                _buildProcess.WaitForExit();
            }
            catch (Exception ex)
            {

            }
            finally {
                Environment.CurrentDirectory = currentDir;
            }

            // Clear the process pointer, so that cancel
            // can run after we've already finished.
            lock (_buildTask)
                _buildProcess = null;
        }

        public void CancelBuild()
        {
            if (_buildTask == null || _buildTask.IsCompleted)
                return;

            lock (_buildTask)
            {
                if (_buildProcess == null)
                    return;
                
                _buildProcess.Kill();
                _buildProcess = null;
            }
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
            if (!ProjectDirty)
                return true;

            // Ask the user if they want to save or cancel.
            var result = View.AskSaveOrCancel();

            // Did we cancel the exit?
            if (result == AskResult.Cancel)
                return false;

            // Did we want to skip saving?
            if (result == AskResult.No)
                return true;

            return SaveProject(false);
        }

        private void UpdateTree()
        {
            /*View.BeginTreeUpdate();

            if (_project == null || string.IsNullOrEmpty(_project.OriginalPath))
                View.SetTreeRoot(null);
            else
            {
                View.SetTreeRoot(_project);

                foreach (var item in _project.ContentItems)
                    View.AddTreeItem(item);
            }            

            View.EndTreeUpdate();*/
        }

        public bool Exit()
        {
            // Can't exit if we're building!
            if (ProjectBuilding)
            {
                View.ShowMessage("You cannot exit while the project is building!");
                return false;
            }

            // Make sure we give the user a chance to
            // save the project if they need too.
            var ret = AskSaveProject();

            if (ret)
                PipelineSettings.Default.Save();

            return ret;
        }

        public IProjectItem GetItem(string originalPath)
        {
            if (_project.OriginalPath.Equals(originalPath, StringComparison.OrdinalIgnoreCase))
                return _project;

            foreach (var i in _project.ContentItems)
            {
                if (string.Equals(i.OriginalPath, originalPath, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return null;
        }

        private void ResolveTypes()
        {
            PipelineTypes.Load(_project);
            foreach (var i in _project.ContentItems)
            {
                i.Observer = this;
                i.ResolveTypes();
            }
        }

        public string GetFullPath(string filePath)
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

        public string GetRelativePath(string path)
        {
            if (!ProjectOpen)
                return path;

            var dirUri = new Uri(ProjectLocation);
            var fileUri = new Uri(path);
            var relativeUri = dirUri.MakeRelativeUri(fileUri);

            if (relativeUri == null)
                return path;

            return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
