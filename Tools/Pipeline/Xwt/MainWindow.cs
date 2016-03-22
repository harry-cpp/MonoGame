using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xwt;
using Xwt.Formats;
using System.IO;

namespace MonoGame.Tools.Pipeline
{
    partial class MainWindow : Window, IView
    {
        public IController _controller;
        FileDialogFilter MonoGameContentProjectFileFilter, XnaContentProjectFileFilter, AllFilesFilter;

        event EventHandler<EventArgs> MenuUpdated;

        public MainWindow ()
        {
            Build ();

            MonoGameContentProjectFileFilter = new FileDialogFilter ("MonoGame Content Build Projects (*.mgcb)", "*.mgcb");
            XnaContentProjectFileFilter = new FileDialogFilter ("XNA Content Projects (*.contentproj)", "*.contentproj");
            AllFilesFilter = new FileDialogFilter ("All Files (*.*)", "*.*");

            MessageDialog.RootWindow = this;

            projectView1.SelectionChanged += (sender, e) => UpdateMenu ();

            this.CloseRequested += MainWindow_CloseRequested;

            outputTextView1.BoundsChanged += OutputTextView1_BoundsChanged;
            projectView1.SelectionChanged += ProjectView1_SelectionChanged;
        }

        public void Open (string [] args)
        {
            var projectPath = PipelineSettings.Default.StartupProject;

            if (args != null && args.Length > 0)
                projectPath = string.Join (" ", args);

            if (!string.IsNullOrEmpty (projectPath))
                _controller.OpenProject (projectPath);
        }

        public void Open (bool loc)
        {
            FileType t;
            string p, l;

            projectView1.GetInfo (projectView1.SelectedRow, out t, out p, out l);

            string path = (loc) ? _controller.GetFullPath (l) : _controller.GetFullPath (p);

            if (t == FileType.Base && !loc)
                path = ((PipelineController)_controller).CurrentProject.OriginalPath;

            Process.Start (path);
        }

        public void RebuildItems ()
        {
            FileType t;
            string p, l;

            projectView1.GetInfo (projectView1.SelectedRow, out t, out p, out l);

            var items = new List<ContentItem> ();

            if (t != FileType.File)
                items = GetChildItems (p);
            else {
                var item = _controller.GetItem (p) as ContentItem;
                if (item != null)
                    items.Add (item);
            }

            _controller.RebuildItems (items);
        }

        public void ShowAboudDialog ()
        {
#if !LINUX
            var adialog = new AboutDialog();
            adialog.TransientFor = this;
#else
            var adialog = new Gtk.AboutDialog ();
            adialog.Logo = new Gdk.Pixbuf (null, "MonoGame.Tools.Pipeline.App.ico");
            adialog.TransientFor = (Gtk.Window)(new Xwt.GtkBackend.GtkEngine ()).GetNativeParentWindow (this.Content);
#endif

            adialog.ProgramName = AssemblyAttributes.AssemblyProduct;
            adialog.Version = AssemblyAttributes.AssemblyVersion;
            adialog.Comments = AssemblyAttributes.AssemblyDescription;
            adialog.Copyright = AssemblyAttributes.AssemblyCopyright;
            adialog.Website = "http://www.monogame.net/";
            adialog.WebsiteLabel = "MonoGame Website";

            adialog.Run ();
#if !LINUX
            adialog.Close();
#else
            adialog.Destroy ();
#endif
        }

        string CombineVariables (string vara, string varb)
        {
            if (vara == "????" || vara == varb)
                return varb;
            return "";
        }

        void ReloadPropertyGrid ()
        {
            string name = "????";
            string location = "????";

            if (!_controller.ProjectOpen) {
                propertyGrid1.Load (null, name, location);
                return;
            }

            _controller.Selection.Clear (this);

            FileType [] t;
            string [] paths, l;

            if (!GetSelection (out t, out paths, out l))
                return;

            var project = ((PipelineController)_controller).CurrentProject;
            bool ps = false;

            var citems = new List<ContentItem> ();
            var dirpaths = new List<string> ();

            for (int i = 0; i < paths.Length; i++) {
                if (t [i] == FileType.Base) {
                    ps = true;
                    name = CombineVariables (name, Path.GetFileNameWithoutExtension (paths [i]));
                    location = CombineVariables (location, project.Location);
                } else {
                    var item = _controller.GetItem (paths [i]);

                    if (item as ContentItem != null) {
                        citems.Add (item as ContentItem);
                        _controller.Selection.Add (item, this);
                    } else
                        dirpaths.Add (paths [i]);

                    name = CombineVariables (name, Path.GetFileNameWithoutExtension (paths [i]));
                    location = CombineVariables (location, Path.GetFileName (Path.GetDirectoryName (paths [i])));
                }
            }

            if (citems.Count > 0 && !ps && dirpaths.Count == 0) {
                var objs = new List<object> ();
                objs.AddRange (citems.ToArray ());
                propertyGrid1.Load (objs, name, location);
            } else if (citems.Count == 0 && ps && dirpaths.Count == 0) {
                var objs = new List<object> ();
                objs.Add (project);
                propertyGrid1.Load (objs, name, location);
            } else
                propertyGrid1.Load (null, name, location);
        }

        protected void ProjectView1_SelectionChanged (object sender, EventArgs e)
        {
            ReloadPropertyGrid ();
        }

        protected void OutputTextView1_BoundsChanged (object sender, EventArgs e)
        {
            scrollView1.VerticalScrollControl.Value = outputTextView1.Size.Height;
        }

        protected void MainWindow_CloseRequested (object sender, CloseRequestedEventArgs args)
        {
            args.AllowClose = _controller.Exit ();

            if (args.AllowClose)
                Application.Exit ();
        }

        #region IView implementation

        public void Attach (IController controller)
        {
            _controller = controller;

            _controller.OnProjectLoading += UpdateMenu;
            _controller.OnProjectLoaded += UpdateMenu;
            _controller.OnBuildStarted += UpdateMenu;
            _controller.OnBuildFinished += () => Application.Invoke (delegate {
                UpdateMenu ();
            });

            _controller.OnCanUndoRedoChanged += UpdateUndoRedo;

            projectView1.Attach (this);
            propertyGrid1.Attach (this);
            UpdateMenu ();
        }

        public void ReloadRecentList (List<string> paths)
        {
            miOpenRecent.SubMenu.Items.Clear ();

            foreach (string path in paths) {
                var item = new MenuItem (path);
                item.Clicked += (sender, e) => _controller.OpenProject (((MenuItem)sender).Label);
                miOpenRecent.SubMenu.Items.Add (item);
            }

            if (paths.Count > 0) {
                var item = new MenuItem ("Clear");
                item.Clicked += (sender, e) => _controller.ClearRecentList ();
                miOpenRecent.SubMenu.Items.Add (new SeparatorMenuItem ());
                miOpenRecent.SubMenu.Items.Add (item);
            } else {
                var item = new MenuItem ("No History");
                item.Sensitive = false;
                miOpenRecent.SubMenu.Items.Add (item);
            }
        }

        public AskResult AskSaveOrCancel ()
        {
            var no = new Command (Command.No.Id, "No");
            var cancel = new Command (Command.Cancel.Id, "Cancel");
            var save = new Command (Command.Yes.Id, "Save");

            var c = MessageDialog.AskQuestion ("Save", "Do you want to save the project first?", new [] { save, no, cancel });

            if (c.Id == save.Id)
                return AskResult.Yes;
            else if (c.Id == no.Id)
                return AskResult.No;

            return AskResult.Cancel;
        }

        public bool AskSaveName (ref string filePath, string title)
        {
            var sfdialog = new SaveFileDialog (title);

            sfdialog.Filters.Add (MonoGameContentProjectFileFilter);
            sfdialog.Filters.Add (AllFilesFilter);

            if (sfdialog.Run (this)) {
                filePath = sfdialog.FileName;

                if (sfdialog.ActiveFilter == MonoGameContentProjectFileFilter && !filePath.EndsWith (".mgcb"))
                    filePath += ".mgcb";

                return true;
            }

            return false;
        }

        public bool AskOpenProject (out string projectFilePath)
        {
            var ofdialog = new OpenFileDialog ("Open MGCB Project");

            ofdialog.Filters.Add (MonoGameContentProjectFileFilter);
            ofdialog.Filters.Add (AllFilesFilter);

            if (ofdialog.Run (this)) {
                projectFilePath = ofdialog.FileName;
                return true;
            }

            projectFilePath = "";
            return false;
        }

        public bool AskImportProject (out string projectFilePath)
        {
            var ofdialog = new OpenFileDialog ("Import XNA Content Project");

            ofdialog.Filters.Add (XnaContentProjectFileFilter);
            ofdialog.Filters.Add (AllFilesFilter);

            if (ofdialog.Run (this)) {
                projectFilePath = ofdialog.FileName;
                return true;
            }

            projectFilePath = "";
            return false;
        }

        public void ShowError (string title, string message)
        {
            MessageDialog.ShowError (message);
        }

        public void ShowMessage (string message)
        {
            MessageDialog.ShowMessage (message);
        }

        public void BeginTreeUpdate ()
        {

        }

        public void SetTreeRoot (IProjectItem item)
        {
            if (item != null)
                projectView1.SetRoot (System.IO.Path.GetFileNameWithoutExtension (item.OriginalPath));
            else
                projectView1.Close ();
        }

        public void ExpandPath (string path)
        {
            projectView1.ExpandPath (projectView1.GetRoot (), path);
        }

        public void AddTreeItem (IProjectItem item)
        {
            projectView1.AddItem (projectView1.GetRoot (), item.OriginalPath, item.Exists, false, item.OriginalPath);
        }

        public void AddTreeFolder (string folder)
        {
            projectView1.AddItem (projectView1.GetRoot (), folder, true, true, folder);
        }

        public void RemoveTreeItem (ContentItem contentItem)
        {
            projectView1.RemoveItem (projectView1.GetRoot (), contentItem.OriginalPath);
        }

        public void RemoveTreeFolder (string folder)
        {
            projectView1.RemoveItem (projectView1.GetRoot (), folder);
        }

        public void UpdateTreeItem (IProjectItem item)
        {

        }

        public void EndTreeUpdate ()
        {

        }

        public void UpdateProperties (IProjectItem item)
        {
            UpdateMenu ();
        }

        string output = "";

        public void OutputAppend (string text)
        {
            Application.Invoke (delegate {
                if (output != "")
                    output += "\r\n";

                output += text;
                outputTextView1.LoadText (output, TextFormat.Plain);
            });
        }

        public void OutputClear ()
        {
            Application.Invoke (delegate {
                output = "";
                outputTextView1.LoadText (output, TextFormat.Plain);
            });
        }

        public bool ChooseContentFile (string initialDirectory, out List<string> files)
        {
            var dialog = new OpenFileDialog ();
            dialog.CurrentFolder = initialDirectory;
            dialog.Filters.Add (AllFilesFilter);
            dialog.Multiselect = true;
            files = new List<string> ();

            if (dialog.Run (this)) {
                files.AddRange (dialog.FileNames);
                return true;
            }
            return false;
        }

        public bool ChooseContentFolder (string initialDirectory, out string folder)
        {
            var dialog = new SelectFolderDialog ();
            dialog.CurrentFolder = initialDirectory;

            if (dialog.Run (this)) {
                folder = dialog.Folder;
                return true;
            }

            folder = "";
            return false;
        }

        public bool ChooseItemTemplate (out ContentItemTemplate template, out string name)
        {
            var dialog = new NewTemplateDialog (_controller.Templates.GetEnumerator ());
            dialog.TransientFor = this;

            Command result = dialog.Run ();
            dialog.Close ();

            if (result == Command.Ok) {
                template = dialog.TemplateFile;
                name = dialog.Name;
                return true;
            }

            template = null;
            name = null;
            return false;
        }

        public bool ChooseName (string title, string text, string oldname, bool docheck, out string newname)
        {
            var dialog = new TextEditDialog (title, text, oldname, docheck);
            dialog.TransientFor = this;

            var result = dialog.Run ();
            dialog.Close ();

            if (result == Command.Ok) {
                newname = dialog.Text;
                return true;
            }

            newname = null;
            return false;
        }

        public bool CopyOrLinkFile (string file, bool exists, out CopyAction action, out bool applyforall)
        {
            var dialog = new AddItemDialog (FileType.File, file, exists);
            dialog.TransientFor = this;

            var result = dialog.Run ();
            dialog.Close ();

            action = dialog.Action;
            applyforall = dialog.ApplyForAll;
            return (result == Command.Ok);
        }

        public bool CopyOrLinkFolder (string folder, bool exists, out CopyAction action, out bool applyforall)
        {
            var dialog = new AddItemDialog (FileType.Folder, folder, exists);
            dialog.TransientFor = this;

            var result = dialog.Run ();
            dialog.Close ();

            action = dialog.Action;
            applyforall = dialog.ApplyForAll;
            return (result == Command.Ok);
        }

        public void OnTemplateDefined (ContentItemTemplate item)
        {

        }

        public Process CreateProcess (string exe, string commands)
        {
            var _buildProcess = new Process ();

            if (Global.OS == OS.Windows) {
                _buildProcess.StartInfo.FileName = exe;
                _buildProcess.StartInfo.Arguments = commands;
            } else {
                _buildProcess.StartInfo.FileName = "mono";
                if (_controller.LaunchDebugger) {
                    var port = Environment.GetEnvironmentVariable ("MONO_DEBUGGER_PORT");
                    port = !string.IsNullOrEmpty (port) ? port : "55555";
                    var monodebugger = string.Format ("--debug --debugger-agent=transport=dt_socket,server=y,address=127.0.0.1:{0}",
                                       port);
                    _buildProcess.StartInfo.Arguments = string.Format ("{0} \"{1}\" {2}", monodebugger, exe, commands);
                    OutputAppend ("************************************************");
                    OutputAppend ("RUNNING MGCB IN DEBUG MODE!!!");
                    OutputAppend (string.Format ("Attach your Debugger to localhost:{0}", port));
                    OutputAppend ("************************************************");
                } else {
                    _buildProcess.StartInfo.Arguments = string.Format ("\"{0}\" {1}", exe, commands);
                }
            }

            return _buildProcess;
        }

        public void ItemExistanceChanged (IProjectItem item)
        {
            Application.Invoke (delegate {
                var tp = projectView1.GetItemFromPath (projectView1.GetRoot (), item.OriginalPath);

                if (!item.Exists)
                    projectView1.SetDoesntExist (tp);
                else
                    projectView1.SetExists (tp);
            });
        }

        public bool GetSelection (out FileType fileType, out string path, out string location)
        {
            var rows = projectView1.SelectedRows;

            if (rows.Length != 1) {
                fileType = FileType.Base;
                path = "";
                location = "";

                return false;
            }

            projectView1.GetInfo (projectView1.SelectedRow, out fileType, out path, out location);
            return true;
        }

        public bool GetSelection (out FileType [] fileType, out string [] path, out string [] location)
        {
            var types = new List<FileType> ();
            var paths = new List<string> ();
            var locs = new List<string> ();

            var rows = projectView1.SelectedRows;

            foreach (var r in rows) {
                FileType t;
                string p, l;

                projectView1.GetInfo (r, out t, out p, out l);

                types.Add (t);
                paths.Add (p);
                locs.Add (l);
            }

            fileType = types.ToArray ();
            path = paths.ToArray ();
            location = locs.ToArray ();

            return (rows.Length > 0);
        }

        public List<ContentItem> GetChildItems (string path)
        {
            return projectView1.GetItems (projectView1.GetItemFromPath (projectView1.GetRoot (), path));
        }

        #endregion

        public void ShowMenu (int x, int y)
        {
            var cmi = _controller.GetContextMenuVisibilityInfo ();

            cmiOpen.Visible = cmi.Open;
            cmiAdd.Visible = cmi.Add;
            cmiAddSeperator.Visible = cmi.Open || cmi.Add;
            cmiOpenDirectory.Visible = cmi.OpenFileLocation;
            cmiRebuild.Visible = cmi.Rebuild;
            cmiRebuildSeparator.Visible = cmi.OpenFileLocation || cmi.Rebuild;
            cmiRename.Visible = cmi.Rename;
            cmiDelete.Visible = cmi.Delete;

            contextMenu.Popup (projectView1, x, y);
        }

        public void UpdateMenu ()
        {
            var ms = _controller.GetMenuSensitivityInfo ();

            miNew.Sensitive = ms.New;
            miOpen.Sensitive = ms.Open;
            miClose.Sensitive = ms.Close;
            miSave.Sensitive = ms.Save;
            miSaveAs.Sensitive = ms.SaveAs;
            miAdd.Sensitive = ms.Add;
            miRename.Sensitive = ms.Rename;
            miDelete.Sensitive = ms.Delete;
            miBuild.Sensitive = ms.Build;
            miRebuild.Sensitive = ms.Rebuild;
            miClean.Sensitive = ms.Clean;
            miDebug.Sensitive = ms.Debug;
            menuBuild.Sensitive = ms.BuildMenu;
            miCancel.Visible = miCancelSeparator.Visible = ms.Cancel;
            UpdateUndoRedo (_controller.CanUndo, _controller.CanRedo);

            if (MenuUpdated != null)
                MenuUpdated (this, new EventArgs ());
        }

        void UpdateUndoRedo (bool canUndo, bool canRedo)
        {
            miUndo.Sensitive = canUndo;
            miRedo.Sensitive = canRedo;
        }
    }
}
