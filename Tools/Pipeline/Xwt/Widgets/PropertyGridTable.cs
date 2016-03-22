using System;
using Xwt;
using System.Collections.Generic;

namespace MonoGame.Tools.Pipeline
{
    public class FalseWidget
    {
        public object newvalue;

        public FalseWidget(object newvalue)
        {
            this.newvalue = newvalue;
        }
    }

    public class EventItem
    {
        public int id;
        public EventHandler eventHandler;

        public EventItem(int id, EventHandler eventHandler)
        {
            this.id = id;
            this.eventHandler = eventHandler;
        }
    }

    public class TreeItem
    {
        public string group;

        public string label;
        public object value;
        public PropertyGridTable.EntryType type;
        public EventHandler eventHandler;
        public Dictionary<string, object> comboItems;

        public TreeItem(string label, object value, PropertyGridTable.EntryType type, EventHandler eventHandler = null, Dictionary<string, object> comboItems = null)
        {
            group = "";

            this.label = label;
            this.value = value;
            this.type = type;
            this.eventHandler = eventHandler;
            this.comboItems = comboItems;
        }
    }

    public class PropertyGridTable : VBox
    {
        private MainWindow window;

        public bool sortgroup;

        public enum EntryType {
            Text,
            Readonly,
            Color,
            Combo,
            Check,
            Single,
            List,
            FilePath,
            Unkown
        }

        public PropertyGridTable()
        {
            sortgroup = false;
        }

        public void Initalize(Window window)
        {
            this.window = (MainWindow)window;
        }

        public void ClearA() 
        {
            this.Clear();
            groups.Clear();
        }

        List<PropertyGridGroup> groups = new List<PropertyGridGroup>();

        private PropertyGridGroup GetGroup(string title)
        {
            foreach (var g in groups)
                if (g.Title == title)
                    return g;

            var pg = new PropertyGridGroup(title);
            groups.Add(pg);
            this.PackStart(pg);

            return pg;
        }

        public void AddEntry(string label, object value, EntryType type, EventHandler eventHandler = null, Dictionary<string, object> comboItems = null) {
            AddEntry(false, label, value, type, eventHandler, comboItems);
        }

        public void AddProcEntry(string label, object value, EntryType type, EventHandler eventHandler = null, Dictionary<string, object> comboItems = null) {
            AddEntry(true, label, value, type, eventHandler, comboItems);
        }

        private void AddEntry(bool subcat, string label, object value, EntryType type, EventHandler eventHandler = null, Dictionary<string, object> comboItems = null)
        {
            string g = "Settings";

            if (label == "Name" || label == "Location")
                g = "Common";

            if (subcat)
                g = "Processor Settings";



            var pg = GetGroup(g);

            switch (type)
            {
                case EntryType.Check:
                    var cb = new CheckBox();
                    cb.Active = (bool)value;

                    cb.Toggled += delegate
                    {
                            if(eventHandler != null)
                                eventHandler(new FalseWidget(cb.Active.ToString()), EventArgs.Empty);
                    };

                    pg.AddProperty(label, cb);
                    break;
                case EntryType.Color:
                    var cp = new ColorPicker();
                    var c = (Microsoft.Xna.Framework.Color)value;
                    cp.Color = new Xwt.Drawing.Color(((double)c.R) / 255, ((double)c.G) / 255, ((double)c.B) / 255, ((double)c.A) / 255);

                    cp.ColorChanged += delegate
                        {
                            if(eventHandler == null)
                                return;
                                
                            var col = new Microsoft.Xna.Framework.Color ();
                            col.R = (byte)Convert.ToInt32(cp.Color.Red * 255);
                            col.G = (byte)Convert.ToInt32(cp.Color.Green * 255);
                            col.B = (byte)Convert.ToInt32(cp.Color.Blue * 255);
                            col.A = (byte)Convert.ToInt32(cp.Color.Alpha * 255);

                            eventHandler(new FalseWidget(col.ToString()), EventArgs.Empty);
                    };

                    pg.AddProperty(label, cp);
                    break;
                case EntryType.Combo:
                    var cob = new ComboBox();
                    cob.MouseScrolled += delegate(object sender, MouseScrolledEventArgs e)
                    {
                            var psv = ((ScrollView)this.Parent);
                            psv.VerticalScrollControl.Value += psv.VerticalScrollControl.StepIncrement * ((e.Direction == ScrollDirection.Up) ? -1 : 1);

                            e.Handled = true;
                    };
                    foreach (var i in comboItems)
                    {
                        cob.Items.Add(i.Key);

                        if (i.Value.ToString() == value.ToString())
                            cob.SelectedText = i.Key;
                    }

                    cob.SelectionChanged += delegate
                    {
                            if(eventHandler != null && cob.SelectedText != null && cob.SelectedText != "")
                                eventHandler(new FalseWidget(cob.SelectedText), EventArgs.Empty);
                    };

                    pg.AddProperty(label, cob);
                    break;
                case EntryType.FilePath:

                    var hbox1 = new HBox();

                    var fpe = new TextEntry();
                    fpe.ShowFrame = false;
                    fpe.WidthRequest = 1;
                    fpe.Text = value.ToString();
                    hbox1.PackStart(fpe, true);

                    fpe.Changed += delegate
                    {
                            if (eventHandler != null)
                                eventHandler(new FalseWidget(fpe.Text), EventArgs.Empty);   
                    };

                    var bb = new Button("...");
                    hbox1.PackStart(bb);

                    bb.Clicked += delegate
                    {
                            var fdialog = new SelectFolderDialog();
                            fdialog.CurrentFolder = window._controller.GetFullPath(fpe.Text);

                            if(fdialog.Run(this.ParentWindow))
                            {
                                string pl = ((PipelineController)window._controller).ProjectLocation;
                                if (!pl.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                                    pl += System.IO.Path.DirectorySeparatorChar;

                                var folderUri = new Uri(pl);
                                var pathUri = new Uri(fdialog.Folder);

                                fpe.Text = Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', System.IO.Path.DirectorySeparatorChar));
                            }
                    };

                    pg.AddProperty(label, hbox1);
                    break;
                case EntryType.Single:
                    var sb = new SpinButton();
                    sb.MinimumValue = -10000;
                    sb.MaximumValue = 10000;
                    sb.Digits = 3;
                    sb.IncrementValue = 0.1;
                    sb.Value = Convert.ToDouble(value);

                    sb.MouseScrolled += delegate(object sender, MouseScrolledEventArgs e)
                        {
                            var psv = ((ScrollView)this.Parent);
                            psv.VerticalScrollControl.Value += psv.VerticalScrollControl.StepIncrement * ((e.Direction == ScrollDirection.Up) ? -1 : 1);

                            e.Handled = true;
                        };
                    
                    sb.ValueChanged += delegate
                    {
                            if (eventHandler != null)
                                eventHandler(new FalseWidget(sb.Value), EventArgs.Empty); 
                    };

                    pg.AddProperty(label, sb);
                    break;
                case EntryType.List:
                    var coll = new Button("Collection");
                    List<string> cvalue = (List<string>)value;

                    coll.Clicked += delegate
                    {
                            var dialog = new ReferenceDialog(window, cvalue);
                            dialog.TransientFor = this.ParentWindow;

                            var result = dialog.Run();
                            dialog.Close();

                            if(result == Command.Ok && eventHandler != null)
                            {
                                cvalue = dialog.References;
                                eventHandler(new FalseWidget(dialog.References), EventArgs.Empty); 
                            }
                    };

                    pg.AddProperty(label, coll);
                    break;
                case EntryType.Readonly:
                    pg.AddProperty(label, new Label(value.ToString()));
                    break;
                case EntryType.Text:
                    var entry = new TextEntry();
                    entry.Text = value.ToString();
                    entry.ShowFrame = false;

                    entry.Changed += delegate
                    {
                            if (eventHandler != null)
                                eventHandler(new FalseWidget(entry.Text), EventArgs.Empty);
                    };

                    pg.AddProperty(label, entry);
                    break;
                case EntryType.Unkown:
                    pg.AddProperty(label, new Label());
                    break;
            }
        }
    }
}

