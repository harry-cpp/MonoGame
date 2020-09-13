// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto.Drawing;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor.Property
{
    public abstract class PropertyCell
    {
        private Rectangle _cellRectangle;
        private object _value;
        private Action<object> _callback;

        public string Category { get; protected set; }
        public string Name { get; protected set; }
        public bool Editable { get; protected set; }

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
        public virtual string DisplayValue => (Value ?? "").ToString();

        public void OnInitialize(string category, string name, object value, bool editable, Action<object> callback)
        {
            _cellRectangle = Rectangle.Empty;
            _value = value;
            _callback = callback;

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
            _cellRectangle.Height = Draw(g, _cellRectangle, selected);
        }

        public virtual void Initialize()
        {

        }

        public abstract Control Edit();

        public virtual int Draw(Graphics g, Rectangle rec, bool selected)
        {
            g.DrawText(
                font: DrawInfo.TextFont,
                color: DrawInfo.GetTextColor(selected, !Editable),
                x: rec.X + 5,
                y: rec.Y + 6,
                text: DisplayValue
            );
            return rec.Height;
        }
    }
}
