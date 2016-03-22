using System;
using Xwt;
using Xwt.Drawing;

namespace MonoGame.Tools.Pipeline
{
    public partial class AboutDialog : Dialog
    {
        public string ProgramName {
            set {
                labelProgramName.Text = value;
            }
        }

        public string Version {
            set {
                labelVersion.Text = value;
            }
        }

        public string Comments {
            set {
                labelComments.Text = value;
            }
        }

        public string Copyright {
            set {
                labelCopyright.Text = value;
            }
        }

        public string Website {
            set {
                labelWebsite.Uri = new Uri (value);
            }
        }

        public string WebsiteLabel {
            set {
                labelWebsite.Text = value;
            }
        }

        public AboutDialog ()
        {
            Build ();
        }
    }
}
