// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Project
{
    public partial class NewFileDialog : Dialog
    {
        private Button _buttonCreate, _buttonCancel;
        private TreeGridView _treeView;
        private TextBox _textBox;

        private void InitializeComponent()
        {
            DisplayMode =  DialogDisplayMode.Attached;
            Resizable = false;
            Size = new Size(300, 400);
            Title = "New File";
            Padding = 8;

            var dynamicLayout = new DynamicLayout();
            dynamicLayout.Spacing = new Eto.Drawing.Size(8, 8);

            _textBox = new TextBox();
            

            _treeView = new TreeGridView();
            _treeView.AllowEmptySelection = false;
            _treeView.AllowMultipleSelection = false;
            _treeView.Columns.Add(new GridColumn
            {
                DataCell = new ImageTextCell(0, 1),
                HeaderText = "Templates"
            });

            // On Linux dialog buttons are on top
            if (Util.IsLinux)
            {
                dynamicLayout.Add(_textBox);
                dynamicLayout.Add(_treeView, true, true);
            }
            else
            {
                dynamicLayout.Add(_treeView, true, true);
                dynamicLayout.Add(_textBox);
            }

            Content = dynamicLayout;

            _buttonCreate = new Button();
            _buttonCreate.Text = "Create File";
            PositiveButtons.Add(_buttonCreate);
            DefaultButton = _buttonCreate;

            _buttonCancel = new Button();
            _buttonCancel.Text = "Cancel";
            NegativeButtons.Add(_buttonCancel);
            AbortButton = _buttonCancel;
        }
    }
}
