using System;
using Xwt;

namespace MonoGame.Tools.Pipeline
{
    public partial class NewTemplateDialog
    {
        /*protected override void OnShown()
        {
            if (!NativeMethods.UseHeaderBar)
                return;
            
            Xwt.GtkBackend.GtkEngine ge = new Xwt.GtkBackend.GtkEngine();

            var gtk_window = (Gtk.Dialog)ge.GetNativeParentWindow(this.Content);

            Gtk.HeaderBar hbar = new Gtk.HeaderBar();
            hbar.Title = this.Title;

            this.Buttons.Remove(dbOk);
            this.Buttons.Remove(dbCancel);

            var ndbOk = new Gtk.Button("Ok");
            ndbOk.Clicked += (sender, e) => this.Respond(Command.Ok);
            ndbOk.Sensitive = dbOk.Sensitive;
            OkEnabled += (sender, e) => ndbOk.Sensitive = dbOk.Sensitive;
            hbar.PackEnd(ndbOk);

            var ndbCancel = new Gtk.Button("Cancel");
            ndbCancel.Clicked += (sender, e) => this.Respond(Command.Cancel);
            hbar.PackStart(ndbCancel);

            gtk_window.Titlebar = hbar;
            this.Height = 500;

            vbox1.Remove(listView1);
            vbox1.PackEnd(listView1, true);

            hbar.ShowAll();
        }*/
    }
}

