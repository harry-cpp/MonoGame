// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Threading;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Project
{
    public partial class FileProgressDialog : Dialog
    {
        private bool _allowExit;
        private Thread _thread;
        private Action _action;

        public FileProgressDialog(Action action)
        {
            InitializeComponent();

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
