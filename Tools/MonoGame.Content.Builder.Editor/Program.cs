// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.CommandLine.Invocation;
using Eto;
using Eto.Forms;
using MonoGame.Tools.Pipeline.Utilities;

namespace MonoGame.Tools.Pipeline
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Styles.Load();

            var app = new Application(Platform.Detect);
            app.Style = "MGCBEditor";

            PipelineSettings.Default.Load();

            var win = new MainWindow();
            var controller = PipelineController.Create(win);

            app.Run(win);
        }
    }
}
