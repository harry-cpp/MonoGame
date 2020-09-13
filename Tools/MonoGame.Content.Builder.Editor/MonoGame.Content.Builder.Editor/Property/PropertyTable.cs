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
    public partial class PropertyTable : Scrollable
    {
        private const int _spacing = 18;
        private const int _separatorWidth = 8;
        private const int _separatorSafeDistance = 30;

        private PropertyPad _propertyPad;
        private PixelLayout _pixel1;
        private Drawable _drawable;
        private CursorType _currentCursor;
        private PropertyCell _selectedCell;
        private List<PropertyCell> _cells;
        private Point _mouseLocation;
        private int _separatorPos, _moveSeparatorAmount;
        private bool _moveSeparator;
        private int _height;
        private bool _skipEdit, _gruop;
        private Cursor _cursorNormal, _cursorResize;

        public PropertyTable(PropertyPad propertyPad)
        {
            _propertyPad = propertyPad;
            BackgroundColor = DrawInfo.BackColor;
            ExpandContentWidth = true;

            _pixel1 = new PixelLayout();
            _pixel1.BackgroundColor = DrawInfo.BackColor;

            _drawable = new Drawable();
            _drawable.Height = 100;
            _pixel1.Add(_drawable, 0, 0);

            Content = _pixel1;

            _drawable.Paint += Drawable_Paint;
            _drawable.MouseDown += Drawable_MouseDown;
            _drawable.MouseUp += Drawable_MouseUp;
            _drawable.MouseMove += Drawable_MouseMove;
            _drawable.MouseLeave += Drawable_MouseLeave;
            SizeChanged += PropertyGridTable_SizeChanged;

            _separatorPos = 100;
            _mouseLocation = new Point(-1, -1);
            _cells = new List<PropertyCell>();
            _moveSeparator = false;
            _skipEdit = false;
            _cursorResize = new Cursor(CursorType.VerticalSplit);
            _cursorNormal = new Cursor(CursorType.Arrow);
        }

        public void Clear()
        {
            _cells.Clear();
            ClearChildren();
        }

        private bool ClearChildren()
        {
            var children = _pixel1.Children.ToList();
            var ret = children.Count > 1;

            foreach (var control in children)
            {
                if (control != _drawable)
                {
                    _pixel1.Remove(control);
                }
            }

            if (ret)
            {
                _drawable.Invalidate();
                _propertyPad.Reload();
            }

            return ret;
        }


        public void AddEntry(string category, string name, object value, PropertyCell cell, bool editable, Action<object> callback)
        {
            cell.OnInitialize(category, name, value, editable, callback);
            _cells.Add(cell);
        }

        public void Update(bool group)
        {
            if (group)
                _cells.Sort((x, y) => string.Compare(x.Category + x.Name, y.Category + y.Name) + (x.Category == "Processor Parameters" ? 100 : 0) + (y.Category == "Processor Parameters" ? -100 : 0));
            else
                _cells.Sort((x, y) => string.Compare(x.Name, y.Name));

            _gruop = group;
            _drawable.Invalidate();
        }

        private void SetCursor(CursorType cursor)
        {
            if (_currentCursor == cursor)
                return;

            _currentCursor = cursor;
            switch (cursor)
            {
                case CursorType.VerticalSplit:
                    _drawable.Cursor = _cursorResize;
                    break;
                default:
                    _drawable.Cursor = Util.IsGtk ? null : _cursorNormal;
                    break;
            }
        }

        private void Drawable_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            DrawInfo.Update(g);

            var rec = new Rectangle(0, 0, _drawable.Width - 1, DrawInfo.TextHeight + _spacing);
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
                    if (_gruop)
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
                _drawable.Height = _height = newHeight;
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
                    _selectedCell.OnEdit(_pixel1);

                    if (Util.IsGtk && _pixel1.Children.Count() > 1)
                    {
                        _pixel1.RemoveAll();
                        _pixel1 = new PixelLayout();
                        _pixel1.Add(_drawable, 0, 0);
                        _selectedCell.OnEdit(_pixel1);
                        Content = _pixel1;
                    }

                    _drawable.Invalidate();
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

            _drawable.Invalidate();
        }

        private void Drawable_MouseLeave(object sender, MouseEventArgs e)
        {
            _mouseLocation = new Point(-1, -1);
            _moveSeparator = false;
            _drawable.Invalidate();
        }

        private void PropertyGridTable_SizeChanged(object sender, EventArgs e)
        {
            SetWidth();
            _drawable.Invalidate();
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
            _drawable.Width = _pixel1.Width = Width - 2;

            foreach (var child in _pixel1.Children)
                if (child != _drawable)
                    child.Width = _drawable.Width - _separatorPos;
#endif
        }
    }
}
