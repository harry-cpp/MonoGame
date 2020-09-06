using System;

using AppKit;
using Eto.Drawing;
using Foundation;
using MonoGame.Content.Builder.Editor;

namespace MonoGame.Content.Builder.Editor.Mac
{
    public partial class ViewController : NSViewController, IView
    {
        private IController _controller;

        public ViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _controller = new Controller(this);
        }

        public void Attach(IController controller, Pad projectPad, Pad propertyPad)
        {
            _controller = controller;

            var projectView = projectPad.Control.ToNative();
            projectView.Frame = new CoreGraphics.CGRect(0, 0, ProjectExplorerBox.Bounds.Size.Width, ProjectExplorerBox.Bounds.Size.Height);
            projectView.AutoresizingMask |= NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
            ProjectExplorerBox.AddSubview(projectView);
        }

        public void UpdateEnabledCommands()
        {

        }

        public Image GetFileIcon(string filePath)
        {
            throw new NotImplementedException();
        }

        public Image GetFolderIcon()
        {
            throw new NotImplementedException();
        }

        public Image GetLinkIcon()
        {
            throw new NotImplementedException();
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

