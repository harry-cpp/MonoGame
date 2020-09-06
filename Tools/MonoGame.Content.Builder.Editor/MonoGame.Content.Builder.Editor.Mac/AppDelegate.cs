using AppKit;
using Foundation;

namespace MonoGame.Content.Builder.Editor.Mac
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application

            HookupCommand(OpenProjectItem, ViewController.ApplicationController.Commands.OpenProject);
        }

        private void HookupCommand(NSMenuItem menuItem, Eto.Forms.Command command)
        {
            command.EnabledChanged += (o, e) => OnCommandEnabledChanged(menuItem, command);
            OnCommandEnabledChanged(menuItem, command);
        }

        private void OnCommandEnabledChanged(NSMenuItem menuItem, Eto.Forms.Command command)
        {
            if (command.Enabled)
            {
                menuItem.Activated += (o, e) => command.Execute();
            }
            else
            {
                menuItem.Action = null;
            }
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
