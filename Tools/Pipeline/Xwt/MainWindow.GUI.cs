using System.Diagnostics;
using Xwt;
using Xwt.Drawing;

namespace MonoGame.Tools.Pipeline
{
    partial class MainWindow
    {
        MenuItem menuFile, menuEdit, menuBuild, menuHelp;
        MenuItem miNew, miOpen, miOpenRecent, miClose, miImport, miSave, miSaveAs, miExit;
        MenuItem miUndo, miRedo, miAdd, miNewItem, miNewFolder, miExistingItem, miExistingFolder, miRename, miDelete;
        MenuItem miBuild, miRebuild, miClean, miCancel;
        SeparatorMenuItem miCancelSeparator;
        CheckBoxMenuItem miDebug;
        MenuItem miHelp, miAbout;

        Menu menu, contextMenu;
        MenuItem cmiOpen, cmiAdd, cmiOpenDirectory, cmiRebuild, cmiRename, cmiDelete;
        SeparatorMenuItem cmiAddSeperator, cmiRebuildSeparator;
        MenuItem cmiNewItem, cmiNewFolder, cmiExistingItem, cmiExistingFolder;

        VPaned vpanned1;
        HPaned hpanned1;
        ScrollView scrollView1;
        RichTextView outputTextView1;
        ProjectView projectView1;
        PropertyGrid propertyGrid1;

        private void Build ()
        {
            this.Title = "MonoGame Pipeline";
            this.Padding = 0;
            this.Icon = Image.FromResource ("MonoGame.Tools.Pipeline.App.ico");
            this.Width = 800;
            this.Height = 600;

            BuildMenu ();
            BuildContextMenu ();

            hpanned1 = new HPaned ();

            vpanned1 = new VPaned ();
            hpanned1.Panel1.Content = vpanned1;

            projectView1 = new ProjectView ();
            vpanned1.Panel1.Content = projectView1;
            vpanned1.Panel1.Resize = true;

            propertyGrid1 = new PropertyGrid ();
            vpanned1.Panel2.Content = propertyGrid1;
            vpanned1.Panel2.Resize = true;

            vpanned1.Position = 250;

            scrollView1 = new ScrollView ();
            outputTextView1 = new RichTextView ();
            scrollView1.Content = outputTextView1;
            hpanned1.Panel2.Content = scrollView1;
            hpanned1.Panel2.Resize = true;
            hpanned1.Position = 200;

            this.Content = hpanned1;
        }

        private void BuildMenu ()
        {
            menu = new Menu ();

            menuFile = new MenuItem ("File");
            menuFile.SubMenu = new Menu ();
            menu.Items.Add (menuFile);

            miNew = new MenuItem ("New...");
            menuFile.SubMenu.Items.Add (miNew);

            miOpen = new MenuItem ("Open...");
            menuFile.SubMenu.Items.Add (miOpen);

            miOpenRecent = new MenuItem ("Open Recent");
            miOpenRecent.SubMenu = new Menu ();
            menuFile.SubMenu.Items.Add (miOpenRecent);

            miClose = new MenuItem ("Close");
            menuFile.SubMenu.Items.Add (miClose);

            menuFile.SubMenu.Items.Add (new SeparatorMenuItem ());

            miImport = new MenuItem ("Import...");
            menuFile.SubMenu.Items.Add (miImport);

            menuFile.SubMenu.Items.Add (new SeparatorMenuItem ());

            miSave = new MenuItem ("Save");
            menuFile.SubMenu.Items.Add (miSave);

            miSaveAs = new MenuItem ("Save As...");
            menuFile.SubMenu.Items.Add (miSaveAs);

            menuFile.SubMenu.Items.Add (new SeparatorMenuItem ());

            miExit = new MenuItem ("Exit");
            menuFile.SubMenu.Items.Add (miExit);

            menuEdit = new MenuItem ("Edit");
            menuEdit.SubMenu = new Menu ();
            menu.Items.Add (menuEdit);

            miUndo = new MenuItem ("Undo");
            menuEdit.SubMenu.Items.Add (miUndo);

            miRedo = new MenuItem ("Redo");
            menuEdit.SubMenu.Items.Add (miRedo);

            menuEdit.SubMenu.Items.Add (new SeparatorMenuItem ());

            miAdd = new MenuItem ("Add");
            miAdd.SubMenu = new Menu ();
            menuEdit.SubMenu.Items.Add (miAdd);

            miNewItem = new MenuItem ("New Item...");
            miAdd.SubMenu.Items.Add (miNewItem);

            miNewFolder = new MenuItem ("New Folder...");
            miAdd.SubMenu.Items.Add (miNewFolder);

            miAdd.SubMenu.Items.Add (new SeparatorMenuItem ());

            miExistingItem = new MenuItem ("Existing Item...");
            miAdd.SubMenu.Items.Add (miExistingItem);

            miExistingFolder = new MenuItem ("Existing Folder...");
            miAdd.SubMenu.Items.Add (miExistingFolder);

            menuEdit.SubMenu.Items.Add (new SeparatorMenuItem ());

            miRename = new MenuItem ("Rename");
            menuEdit.SubMenu.Items.Add (miRename);

            miDelete = new MenuItem ("Delete");
            menuEdit.SubMenu.Items.Add (miDelete);

            menuBuild = new MenuItem ("Build");
            menuBuild.SubMenu = new Menu ();
            menu.Items.Add (menuBuild);

            miBuild = new MenuItem ("Build");
            menuBuild.SubMenu.Items.Add (miBuild);

            miRebuild = new MenuItem ("Rebuild");
            menuBuild.SubMenu.Items.Add (miRebuild);

            miClean = new MenuItem ("Clean");
            menuBuild.SubMenu.Items.Add (miClean);

            menuBuild.SubMenu.Items.Add (new SeparatorMenuItem ());

            miDebug = new CheckBoxMenuItem ("Debug Mode");
            menuBuild.SubMenu.Items.Add (miDebug);

            miCancelSeparator = new SeparatorMenuItem ();
            menuBuild.SubMenu.Items.Add (miCancelSeparator);

            miCancel = new MenuItem ("Cancel");
            menuBuild.SubMenu.Items.Add (miCancel);

            menuHelp = new MenuItem ("Help");
            menuHelp.SubMenu = new Menu ();
            menu.Items.Add (menuHelp);

            miHelp = new MenuItem ("View Help");
            menuHelp.SubMenu.Items.Add (miHelp);

            menuHelp.SubMenu.Items.Add (new SeparatorMenuItem ());

            miAbout = new MenuItem ("About");
            menuHelp.SubMenu.Items.Add (miAbout);

            this.MainMenu = menu;

            miNew.Clicked += (sender, e) => _controller.NewProject ();
            miOpen.Clicked += (sender, e) => _controller.OpenProject ();
            miClose.Clicked += (sender, e) => _controller.CloseProject ();
            miImport.Clicked += (sender, e) => _controller.ImportProject ();
            miSave.Clicked += (sender, e) => _controller.SaveProject (false);
            miSaveAs.Clicked += (sender, e) => _controller.SaveProject (true);
            miExit.Clicked += (sender, e) => this.Close ();

            miUndo.Clicked += (sender, e) => _controller.Undo ();
            miRedo.Clicked += (sender, e) => _controller.Redo ();

            miNewItem.Clicked += (sender, e) => _controller.NewItem ();
            miNewFolder.Clicked += (sender, e) => _controller.NewFolder ();
            miExistingItem.Clicked += (sender, e) => _controller.Include ();
            miExistingFolder.Clicked += (sender, e) => _controller.IncludeFolder ();
            miRename.Clicked += (sender, e) => _controller.Rename ();
            miDelete.Clicked += (sender, e) => _controller.Delete ();

            miBuild.Clicked += (sender, e) => _controller.Build (false);
            miRebuild.Clicked += (sender, e) => _controller.Build (true);
            miClean.Clicked += (sender, e) => _controller.Clean ();
            miDebug.Clicked += (sender, e) => _controller.LaunchDebugger = miDebug.Checked;
            miCancel.Clicked += (sender, e) => _controller.CancelBuild ();

            miHelp.Clicked += (sender, e) => Process.Start ("http://www.monogame.net/documentation/?page=Pipeline");
            miAbout.Clicked += (sender, e) => ShowAboudDialog ();

            //update menus
            miSave.Clicked += (sender, e) => UpdateMenu ();
            miSaveAs.Clicked += (sender, e) => UpdateMenu ();
            miNewItem.Clicked += (sender, e) => UpdateMenu ();
            miNewFolder.Clicked += (sender, e) => UpdateMenu ();
            miExistingItem.Clicked += (sender, e) => UpdateMenu ();
            miExistingFolder.Clicked += (sender, e) => UpdateMenu ();
            miRename.Clicked += (sender, e) => UpdateMenu ();
            miDelete.Clicked += (sender, e) => UpdateMenu ();
            miBuild.Clicked += (sender, e) => UpdateMenu ();
            miRebuild.Clicked += (sender, e) => UpdateMenu ();
            miClean.Clicked += (sender, e) => UpdateMenu ();
            miCancel.Clicked += (sender, e) => UpdateMenu ();
        }

        private void BuildContextMenu ()
        {
            contextMenu = new Menu ();

            cmiOpen = new MenuItem ("Open");
            contextMenu.Items.Add (cmiOpen);

            cmiAdd = new MenuItem ("Add");
            cmiAdd.SubMenu = new Menu ();
            contextMenu.Items.Add (cmiAdd);

            cmiNewItem = new MenuItem ("New Item");
            cmiAdd.SubMenu.Items.Add (cmiNewItem);

            cmiNewFolder = new MenuItem ("New Folder");
            cmiAdd.SubMenu.Items.Add (cmiNewFolder);

            cmiAdd.SubMenu.Items.Add (new SeparatorMenuItem ());

            cmiExistingItem = new MenuItem ("Existing Item");
            cmiAdd.SubMenu.Items.Add (cmiExistingItem);

            cmiExistingFolder = new MenuItem ("Add Existing Folder");
            cmiAdd.SubMenu.Items.Add (cmiExistingFolder);

            cmiAddSeperator = new SeparatorMenuItem ();
            contextMenu.Items.Add (cmiAddSeperator);

            cmiOpenDirectory = new MenuItem ("Open Item Directory");
            contextMenu.Items.Add (cmiOpenDirectory);

            cmiRebuild = new MenuItem ("Rebuild");
            contextMenu.Items.Add (cmiRebuild);

            cmiRebuildSeparator = new SeparatorMenuItem ();
            contextMenu.Items.Add (cmiRebuildSeparator);

            cmiRename = new MenuItem ("Rename");
            contextMenu.Items.Add (cmiRename);

            cmiDelete = new MenuItem ("Delete");
            contextMenu.Items.Add (cmiDelete);

            cmiOpen.Clicked += (sender, e) => Open (false);

            cmiNewItem.Clicked += (sender, e) => _controller.NewItem ();
            cmiNewFolder.Clicked += (sender, e) => _controller.NewFolder ();
            cmiExistingItem.Clicked += (sender, e) => _controller.Include ();
            cmiExistingFolder.Clicked += (sender, e) => _controller.IncludeFolder ();

            cmiOpenDirectory.Clicked += (sender, e) => Open (true);
            cmiRebuild.Clicked += (sender, e) => RebuildItems ();

            cmiRename.Clicked += (sender, e) => _controller.Rename ();
            cmiDelete.Clicked += (sender, e) => _controller.Delete ();

            //updatemenus
            cmiNewItem.Clicked += (sender, e) => UpdateMenu ();
            cmiNewFolder.Clicked += (sender, e) => UpdateMenu ();
            cmiExistingItem.Clicked += (sender, e) => UpdateMenu ();
            cmiExistingFolder.Clicked += (sender, e) => UpdateMenu ();

            cmiRename.Clicked += (sender, e) => UpdateMenu ();
            cmiDelete.Clicked += (sender, e) => UpdateMenu ();
        }
    }
}
