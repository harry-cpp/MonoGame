using Gtk;

namespace MonoGame.Content.Builder.Editor.Linux
{
    class GtkPad : Widget
    {
        [Gtk.Builder.Object("label_title")]
        private Label _labelTitle = null!;

        [Gtk.Builder.Object("box_content")]
        private Box _boxContent = null!;

        [Gtk.Builder.Object("popover_settings")]
        private PopoverMenu _popoverSettings = null!;

        public GtkPad(Pad pad) : this(pad, new Gtk.Builder("Pad.glade")) { }

        private GtkPad(Pad pad, Gtk.Builder builder) : base(builder.GetObject("pad").Handle)
        {
            builder.Autoconnect(this);

            _labelTitle.Text = pad.Title;
            _boxContent.PackStart(pad.Control.ToNative(), true, true, 0);

            var vbox = new VBox();
            vbox.Margin = 8;

            foreach (var cmd in pad.ViewCommands)
            {
                Widget widget = null;

                if (cmd is Eto.Forms.RadioCommand radioCommand)
                    widget = ConnectCommand(radioCommand);
                else if (cmd is Eto.Forms.CheckCommand checkCommand)
                    widget = ConnectCommand(checkCommand);
                else if (cmd is Eto.Forms.Command command)
                    widget = ConnectCommand(command);
                else
                    widget = new Separator(Orientation.Vertical);

                vbox.PackStart(widget, false, false, 0);
            }

            vbox.ShowAll();
            _popoverSettings.Child = vbox;
        }

        private Widget ConnectCommand(Eto.Forms.CheckCommand command)
        {
            var ret = new ModelButton();
            ret.Role = command is Eto.Forms.RadioCommand ? ButtonRole.Radio : ButtonRole.Check;
            ret.Text = command.MenuText;
            ret.Active = command.Checked;

            command.EnabledChanged += (o, e) => ret.Sensitive = command.Enabled;
            command.CheckedChanged += (o, e) => ret.Active = command.Checked;
            ret.Clicked += (o, e) => command.Checked = true;

            return ret;
        }

        private Widget ConnectCommand(Eto.Forms.Command command)
        {
            var ret = new ModelButton();
            ret.Text = command.MenuText;

            command.EnabledChanged += (o, e) => ret.Sensitive = command.Enabled;
            ret.Clicked += (o, e) => command.Execute();

            return ret;
        }
    }
}
