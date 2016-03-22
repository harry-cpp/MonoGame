// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Diagnostics;
using Xwt;

namespace MonoGame.Tools.Pipeline
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string [] args)
        {
#if WINDOWS
            Application.Initialize(ToolkitType.Wpf);
#else
            Application.Initialize(ToolkitType.Gtk3);
#endif

            PipelineSettings.Default.Load ();

            var window = new MainWindow();
            new PipelineController(window);
            window.Show();

            if (args != null && args.Length > 0)
            {
                // var projectFilePath = string.Join(" ", args);
                window.Open (args);
            }

            Application.Run();
        }
    }
}
