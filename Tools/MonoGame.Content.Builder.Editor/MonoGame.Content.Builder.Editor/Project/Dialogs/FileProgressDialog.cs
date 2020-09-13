// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Threading;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Project
{
    public partial class FileProgressDialog : Dialog
    {
        private bool _allowExit;
        private Thread _thread;
        private Action _action;
        private Label _labelText;

        public FileProgressDialog(Action action)
        {
            Title = "File Operation in Progress";
            Size = new Size(400, -1);
            Padding = 8;
            Resizable = false;

            var layout = new DynamicLayout();
            layout.DefaultSpacing = new Size(4, 4);

            _labelText = new Label();
            _labelText.Text = "Initializing...";
            layout.Add(_labelText, true, false);

            var progressBar = new ProgressBar();
            progressBar.Indeterminate = true;
            layout.Add(progressBar, true, false);

            Content = layout;

            _action = action;
            _thread = new Thread(new ThreadStart(FileOperationThread));

            Shown += (o, e) => _thread?.Start();
            Closing += (o, e) => e.Cancel = !_allowExit;
        }

        public bool IsSuccess;

        private void FileOperationThread()
        {
            _thread = null;

            try
            {
                _action.Invoke();
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                Application.Instance.Invoke(() => MessageBox.Show(ex.ToString(), "An error occured during the file operation.", MessageBoxType.Error));
            }

            _allowExit = true;
            Application.Instance.Invoke(() => Close());
        }

        public void SetText(string text)
        {
            Application.Instance.Invoke(() => _labelText.Text = text);
        }
    }
}
