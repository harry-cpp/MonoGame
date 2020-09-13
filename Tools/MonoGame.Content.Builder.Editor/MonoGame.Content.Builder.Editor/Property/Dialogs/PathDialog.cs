// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
/*
using System;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Tools.Pipeline
{
    partial class PathDialog : Dialog<bool>
    {
        DynamicLayout layout1;
        StackLayout stack1, stack2;
        Label label1, label2;
        TextBox textBox1;
        Button buttonBrowse;
        Button buttonOk, buttonCancel;

        public string Path { get; set; }

        private readonly string[] symbols = { "Platform", "Configuration", "Config", "Profile" };

        public PathDialog(string path)
        {
            Title = "Select Folder";
            DisplayMode = DialogDisplayMode.Attached;
            Size = new Size(400, 200);

            buttonOk = new Button();
            buttonOk.Text = "Ok";
            PositiveButtons.Add(buttonOk);
            DefaultButton = buttonOk;

            buttonCancel = new Button();
            buttonCancel.Text = "Cancel";
            NegativeButtons.Add(buttonCancel);
            AbortButton = buttonCancel;

            layout1 = new DynamicLayout();
            layout1.DefaultSpacing = new Size(4, 4);
            layout1.Padding = new Padding(6);
            layout1.BeginVertical();

            layout1.Add(null, true, true);

            label1 = new Label();
            label1.Text = "Path to use:";
            layout1.Add(label1);

            stack1 = new StackLayout();
            stack1.Spacing = 4;
            stack1.Orientation = Orientation.Horizontal;

            textBox1 = new TextBox();
            stack1.Items.Add(new StackLayoutItem(textBox1, VerticalAlignment.Center, true));

            buttonBrowse = new Button();
            buttonBrowse.Text = "...";
            buttonBrowse.MinimumSize = new Size(1, 1);
            stack1.Items.Add(new StackLayoutItem(buttonBrowse, VerticalAlignment.Center, false));

            layout1.Add(stack1);

            label2 = new Label();
            label2.Text = "Macros:";
            layout1.Add(label2);

            stack2 = new StackLayout();
            stack2.Spacing = 4;
            stack2.Orientation = Orientation.Horizontal;

            foreach (var symbol in symbols)
            {
                var buttonSymbol = new Button();
                buttonSymbol.Text = symbol;
                buttonSymbol.Click += ButtonSymbol_Click;
                stack2.Items.Add(new StackLayoutItem(buttonSymbol, true));
            }

            layout1.Add(stack2);

            layout1.Add(null, true, true);

            Content = layout1;

            textBox1.TextChanged += TextBox1_TextChanged;
            buttonBrowse.Click += ButtonBrowse_Click;
            buttonOk.Click += ButtonOk_Click;
            buttonCancel.Click += ButtonCancel_Click;

            textBox1.Text = path;
        }

        private void ButtonSymbol_Click(object sender, EventArgs e)
        {
            var text = "$(" + (sender as Button).Text + ")";
            int carret;

            if (!string.IsNullOrEmpty(textBox1.SelectedText))
            {
                carret = textBox1.Selection.Start;
                textBox1.Text = textBox1.Text.Remove(carret, textBox1.Selection.End + 1 - carret);
            }
            else
                carret = textBox1.CaretIndex;

            textBox1.Text = textBox1.Text.Insert(carret, text);
            textBox1.Focus();
            textBox1.CaretIndex = carret + text.Length;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            Path = textBox1.Text;
            DefaultButton.Enabled = !string.IsNullOrWhiteSpace(textBox1.Text);
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            /*
            var dialog = new SelectFolderDialog();
            dialog.Directory = _controller.GetFullPath(textBox1.Text);

            if (dialog.Show(this) == DialogResult.Ok)
                textBox1.Text = _controller.GetRelativePath(dialog.Directory);*
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            Result = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}*/