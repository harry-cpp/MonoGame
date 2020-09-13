using System;

using AppKit;
using Eto.Drawing;
using Eto.Mac.Drawing;
using Foundation;
using MonoGame.Content.Builder.Editor;

namespace MonoGame.Content.Builder.Editor.Mac
{
    public partial class ViewController : NSViewController, IView
    {
        public static IController ApplicationController;

        private IController _controller;

        public ViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Util.IsMac = true;
            Util.IsXamarinMac = true;

            ApplicationController = new Controller(this);
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();

            Util.MainWindow = Eto.Forms.XamMac2Helpers.ToEtoWindow(View.Window);
        }

        public void Attach(IController controller, Pad projectPad, Pad propertyPad)
        {
            ApplicationController = controller;
            _controller = controller;

            var projectView = projectPad.Control.ToNative();
            var parentView = ProjectExplorerBox.Subviews[0];
            projectView.Frame = new CoreGraphics.CGRect(0, 0, parentView.Bounds.Size.Width, parentView.Bounds.Size.Height);
            projectView.AutoresizingMask |= NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
            parentView.AddSubview(projectView);

            var propertiesView = propertyPad.Control.ToNative();
            parentView = PropertiesBox.Subviews[0];
            propertiesView.Frame = new CoreGraphics.CGRect(0, 0, parentView.Bounds.Size.Width, parentView.Bounds.Size.Height);
            propertiesView.AutoresizingMask |= NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
            parentView.AddSubview(propertiesView);
        }

        public void UpdateEnabledCommands()
        {

        }

        public Bitmap GetFileIcon(string filePath)
        {
            return new Bitmap(new BitmapHandler(NSWorkspace.SharedWorkspace.IconForFile(filePath)));
        }

        public Bitmap GetFolderIcon()
        {
            return new Bitmap(new BitmapHandler(NSWorkspace.SharedWorkspace.IconForFile("/Library/Apple")));
        }

        public Bitmap GetLinkIcon()
        {
            // TODO
            return new Bitmap(new BitmapHandler(NSWorkspace.SharedWorkspace.IconForFile("/Library/Apple")));
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}

