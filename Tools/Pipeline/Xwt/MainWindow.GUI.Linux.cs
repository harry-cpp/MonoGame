using System;
using Gtk;

namespace MonoGame.Tools.Pipeline
{
    partial class MainWindow
    {
        /*HeaderBar hbar;

        [Builder.ObjectAttribute] Button new_button;
        [Builder.ObjectAttribute] Button open_button;
        [Builder.ObjectAttribute] Button save_button;
        [Builder.ObjectAttribute] Button build_button;
        [Builder.ObjectAttribute] MenuButton gear_button;

        protected override void OnShown()
        {
            if (!NativeMethods.UseHeaderBar)
                return;

            Xwt.GtkBackend.GtkEngine ge = new Xwt.GtkBackend.GtkEngine();
            var gtk_window = (Window)ge.GetNativeParentWindow(this.Content);

            Builder builder = new Builder(null, "MonoGame.Tools.Pipeline.Xwt.MainWindow.HeaderBar.glade", null);
            hbar = new HeaderBar(builder.GetObject("headerbar").Handle);
            builder.Autoconnect(this);

            gtk_window.Titlebar = hbar;
            hbar.ShowCloseButton = true;

            MenuUpdated += (sender, e) => MainWindow_MenuUpdated();

            new_button.Clicked += (sender, e) => _controller.NewProject();
            open_button.Clicked += (sender, e) => _controller.OpenProject();
            save_button.Clicked += (sender, e) => _controller.SaveProject(false);
            build_button.Clicked += (sender, e) => _controller.Build(false);

            save_button.Clicked += (sender, e) => UpdateMenu();
            build_button.Clicked += (sender, e) => UpdateMenu();

            MainMenu = new Xwt.Menu();

            Xwt.MenuButton mb = new Xwt.MenuButton();
            gear_button.Sensitive = true;
            hbar.Remove(gear_button);

            var native_mb = (Button)ge.GetNativeWidget(mb);
            native_mb.Image = gear_button.Image;

            mb.CanGetFocus = false;
            mb.Menu = menu;
            hbar.PackEnd((Widget)ge.GetNativeWidget(mb));
            hbar.ChildSetProperty((Widget)ge.GetNativeWidget(mb), "position", new GLib.Value(0));

            hbar.Show();
            MainWindow_MenuUpdated();
        }

        protected void MainWindow_MenuUpdated ()
        {
            new_button.Sensitive = miNew.Sensitive;
            open_button.Sensitive = miOpen.Sensitive;
            save_button.Sensitive = miSave.Sensitive;
            build_button.Sensitive = miBuild.Sensitive;
        }*/
    }
}

