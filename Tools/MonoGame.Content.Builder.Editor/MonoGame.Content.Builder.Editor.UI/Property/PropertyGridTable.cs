// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    public partial class PropertyPadTable : Scrollable
    {
        private const int _spacing = 18;
        private const int _separatorWidth = 8;
        private const int _separatorSafeDistance = 30;

        public bool Group { get; set; }

        private PropertyPad _propertyPad;
        PixelLayout pixel1;
        Drawable drawable;
        private CursorType _currentCursor;
        private ICell _selectedCell;
        private List<ICell> _cells;
        private Point _mouseLocation;
        private int _separatorPos, _moveSeparatorAmount;
        private bool _moveSeparator;
        private int _height;
        private bool _skipEdit;
        private Cursor _cursorNormal, _cursorResize;

        public PropertyPadTable(PropertyPad propertyPad)
        {
            _propertyPad = propertyPad;
            BackgroundColor = DrawInfo.BackColor;
            ExpandContentWidth = true;

            pixel1 = new PixelLayout();
            pixel1.BackgroundColor = DrawInfo.BackColor;

            drawable = new Drawable();
            drawable.Height = 100;
            pixel1.Add(drawable, 0, 0);

            Content = pixel1;

            drawable.Paint += Drawable_Paint;
            drawable.MouseDown += Drawable_MouseDown;
            drawable.MouseUp += Drawable_MouseUp;
            drawable.MouseMove += Drawable_MouseMove;
            drawable.MouseLeave += Drawable_MouseLeave;
            SizeChanged += PropertyGridTable_SizeChanged;

            _separatorPos = 100;
            _mouseLocation = new Point(-1, -1);
            _cells = new List<ICell>();
            _moveSeparator = false;
            _skipEdit = false;
            _cursorResize = new Cursor(CursorType.VerticalSplit);
            _cursorNormal = new Cursor(CursorType.Arrow);

            Group = true;
        }

        public void Clear()
        {
            _cells.Clear();
            ClearChildren();
        }

        private bool ClearChildren()
        {
            var children = pixel1.Children.ToList();
            var ret = children.Count > 1;

            foreach (var control in children)
            {
                if (control != drawable && control.Tag is ICell cell)
                {
                    pixel1.Remove(control);
                }
            }

            if (ret)
            {
                drawable.Invalidate();
            }

            return ret;
        }

        private Type GetCellType(Type type)
        {
            if (type == typeof(string))
                return typeof(CellString);

            return null;
        }

        public void AddEntry(string category, string name, object value, Type type, bool editable, Action<object> callback)
        {
            var cellType = GetCellType(type);

            var cell = (cellType == null) ? new CellString() : (ICell)Activator.CreateInstance(cellType);
            cell.OnInitialize(_propertyPad, category, name, value, cellType == null ? false : editable, callback);

            _cells.Add(cell);
        }

        public void Update()
        {
            if (Group)
                _cells.Sort((x, y) => string.Compare(x.Category + x.Name, y.Category + y.Name) + (x.Category == "Processor Parameters" ? 100 : 0) + (y.Category == "Processor Parameters" ? -100 : 0));
            else
                _cells.Sort((x, y) => string.Compare(x.Name, y.Name) + (x.Category == "Processor Parameters" ? 100 : 0) + (y.Category == "Processor Parameters" ? -100 : 0));

            drawable.Invalidate();
        }

        private void SetCursor(CursorType cursor)
        {
            if (_currentCursor == cursor)
                return;

            _currentCursor = cursor;
            switch (cursor)
            {
                case CursorType.VerticalSplit:
                    drawable.Cursor = _cursorResize;
                    break;
                default:
                    drawable.Cursor = Util.IsGtk ? null : _cursorNormal;
                    break;
            }
        }

        private void Drawable_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DrawInfo.SetPixelsPerPoint(g);

            var rec = new Rectangle(0, 0, drawable.Width - 1, DrawInfo.TextHeight + _spacing);
            var overGroup = false;
            var prevCategory = string.Empty;

            _separatorPos = Math.Min(Width - _separatorSafeDistance, Math.Max(_separatorSafeDistance, _separatorPos));
            _selectedCell = null;

            g.Clear(DrawInfo.BackColor);

            foreach (var c in _cells)
            {
                rec.Height = (c.Height == 0) ? (DrawInfo.TextHeight + _spacing) : c.Height;

                // Draw group
                if (prevCategory != c.Category)
                {
                    if (Group)
                    {
                        g.FillRectangle(DrawInfo.BorderColor, rec);
                        g.DrawText(DrawInfo.TextFont, DrawInfo.TextColor, rec.X + 1, rec.Y + (rec.Height - DrawInfo.TextFont.LineHeight) / 2, c.Category);

                        prevCategory = c.Category;
                        overGroup |= rec.Contains(_mouseLocation);
                        rec.Y += DrawInfo.TextHeight + _spacing;
                    }
                }

                // Draw cell
                var selected = rec.Contains(_mouseLocation);
                if (selected)
                    _selectedCell = c;
                c.OnDraw(g, rec, _separatorPos, selected);

                // Draw separator for the current row
                g.FillRectangle(DrawInfo.BorderColor, _separatorPos - 1, rec.Y, 1, c.Height);

                rec.Y += c.Height;
            }

            // Draw separator for not filled rows
            g.FillRectangle(DrawInfo.BorderColor, _separatorPos - 1, rec.Y, 1, Height);

            // Set Height
            var newHeight = Math.Max(rec.Y + 1, Height - 2);
            if (_height != newHeight)
            {
                drawable.Height = _height = newHeight;
                SetWidth();
            }

            if (overGroup) // TODO: Group collapsing/expanding?
                SetCursor(CursorType.Arrow);
            else if ((new Rectangle(_separatorPos - _separatorWidth / 2, 0, _separatorWidth, Height)).Contains(_mouseLocation))
                SetCursor(CursorType.VerticalSplit);
            else
                SetCursor(CursorType.Arrow);
        }

        private void Drawable_MouseDown(object sender, MouseEventArgs e)
        {
            _skipEdit = ClearChildren();
            if (_currentCursor == CursorType.VerticalSplit)
            {
                _moveSeparator = true;
                _moveSeparatorAmount = (int)e.Location.X - _separatorPos;
            }
        }

        private void Drawable_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_moveSeparator && e.Location.X >= _separatorPos && _selectedCell != null && _selectedCell.Editable && !_skipEdit)
            {
                var action = new Action(() =>
                {
                    if (Util.IsGtk)
                    {
                        pixel1.RemoveAll();
                        pixel1 = new PixelLayout();
                        pixel1.Add(drawable, 0, 0);
                        _selectedCell.OnEdit(pixel1);
                        Content = pixel1;
                    }
                    else
                    {
                        _selectedCell.OnEdit(pixel1);
                    }

                    drawable.Invalidate();
                });

#if WINDOWS
                (drawable.ControlObject as System.Windows.Controls.Canvas).Dispatcher.BeginInvoke(action,
                    System.Windows.Threading.DispatcherPriority.ContextIdle, null);
#else
                action.Invoke();
#endif
            }
            else
            {
                _moveSeparator = false;
            }
        }

        private void Drawable_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseLocation = new Point((int)e.Location.X, (int)e.Location.Y);

            if (_moveSeparator)
                _separatorPos = _moveSeparatorAmount + _mouseLocation.X;

            drawable.Invalidate();
        }

        private void Drawable_MouseLeave(object sender, MouseEventArgs e)
        {
            _mouseLocation = new Point(-1, -1);
            _moveSeparator = false;
            drawable.Invalidate();
        }

        private void PropertyGridTable_SizeChanged(object sender, EventArgs e)
        {
            SetWidth();
            drawable.Invalidate();
        }

        public void SetWidth()
        {
#if WINDOWS
            var action = new Action(() =>
            {
                var scrollsize = (_height >= Height) ? System.Windows.SystemParameters.VerticalScrollBarWidth : 0.0;
                drawable.Width = pixel1.Width = (int)(Width - scrollsize - System.Windows.SystemParameters.BorderWidth * 2);

                foreach (var child in pixel1.Children)
                    if (child != drawable)
                        child.Width = drawable.Width - _separatorPos;
            });

            (drawable.ControlObject as System.Windows.Controls.Canvas).Dispatcher.BeginInvoke(action,
                System.Windows.Threading.DispatcherPriority.ContextIdle, null);
#else
            drawable.Width = pixel1.Width = Width - 2;

            foreach (var child in pixel1.Children)
                if (child != drawable)
                    child.Width = drawable.Width - _separatorPos;
#endif
        }
    }
}
