// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    public abstract class PropertyCell
    {
        private Rectangle _cellRectangle;
        private object _value;
        private Action<object> _callback;

        public List<object> ParentObjects { get; private set; }
        public string Category { get; private set; }
        public string Name { get; private set; }
        public bool Editable { get; private set; }

        public int Height => _cellRectangle.Height;
        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                _callback.Invoke(value);
            }
        }

        public void OnInitialize(List<object> parentObjects, string category, string name, object value, bool editable, Action<object> callback)
        {
            _cellRectangle = Rectangle.Empty;
            _value = value;
            _callback = callback;

            ParentObjects = parentObjects;
            Category = category;
            Name = name;
            Editable = editable;

            Initialize();
        }

        public void OnEdit(PixelLayout layout)
        {
            var control = Edit();

            if (control != null)
            {
                control.Width = _cellRectangle.Width;
                control.Height = _cellRectangle.Height;

                layout.Add(control, _cellRectangle.X, _cellRectangle.Y);
            }
        }

        public void OnDraw(Graphics g, Rectangle rec, int separatorPos, bool selected)
        {
            if (selected)
                g.FillRectangle(DrawInfo.HoverBackColor, rec);

            g.DrawText(DrawInfo.TextFont, DrawInfo.GetTextColor(selected, false), rec.X + 5, rec.Y + 6, Name);
            g.FillRectangle(DrawInfo.GetBackgroundColor(selected), separatorPos - 6, rec.Y, rec.Width, rec.Height);

            _cellRectangle = rec;
            _cellRectangle.X += separatorPos;
            _cellRectangle.Width -= separatorPos - 1;
            _cellRectangle.Height = DrawCell(g, _cellRectangle, (Value ?? "").ToString(), selected);
        }

        public virtual void Initialize()
        {

        }

        public abstract Control Edit();

        public virtual int DrawCell(Graphics g, Rectangle rec, string displayValue, bool selected)
        {
            g.DrawText(
                font: DrawInfo.TextFont,
                color: DrawInfo.GetTextColor(selected, !Editable),
                x: rec.X + 5,
                y: rec.Y + 6,
                text: displayValue
            );
            return rec.Height;
        }
    }
}
