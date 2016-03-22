using System;
using Xwt;

namespace MonoGame.Tools.Pipeline
{
    public class PropertyGridGroup: VBox
    {
        public string Title;

        HPaned hpaned1;
        VBox vbox1, vbox2;

        public PropertyGridGroup(string title)
        {
            this.Title = title;

            var l = new Label();
            l.Markup = "<b><u>"+title+"</u></b>";
            this.PackStart(l);

            hpaned1 = new HPaned();
            this.PackStart(hpaned1);

            hpaned1.Panel1.Content = vbox1 = new VBox();
            hpaned1.Panel2.Content = vbox2 = new VBox();

            vbox1.MarginLeft = 3;
            vbox2.MarginLeft = 3;

            hpaned1.Panel2.Content.BackgroundColor = new Xwt.Drawing.Color(255, 255, 255);
            hpaned1.Panel2.Resize = true;
        }

        private ScrollView Capsule(Widget widget)
        {
            var sw = new ScrollView();

            sw.Content = widget;
            sw.HeightRequest = 35;
            sw.BorderVisible = false;
            sw.WidthRequest = 1;

            sw.VerticalScrollPolicy = ScrollPolicy.Never;
            sw.HorizontalScrollPolicy = ScrollPolicy.Never;

            return sw;
        }

        public void AddProperty(string label, Widget widget)
        {
            vbox1.PackStart(Capsule(new Label(label + ": ")));
            vbox2.PackStart(Capsule(widget));
            hpaned1.Position = 100;
        }
    }
}

