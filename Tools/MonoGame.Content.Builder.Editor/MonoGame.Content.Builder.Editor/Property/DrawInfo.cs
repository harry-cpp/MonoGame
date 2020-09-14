// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Eto.Drawing;

namespace MonoGame.Content.Builder.Editor.Property
{
    /// <summary>
    /// This class contains the color, size and font information for drawing the property pad.
    /// </summary>
    public static class DrawInfo
    {
        static DrawInfo()
        {
            TextFont = SystemFonts.Default();
            TextHeight = (int)SystemFonts.Default().LineHeight;
            TextColor = SystemColors.ControlText;
            BackColor = SystemColors.ControlBackground;
            HoverTextColor = SystemColors.HighlightText;
            HoverBackColor = SystemColors.Highlight;
            DisabledTextColor = new Color(SystemColors.ControlText, 0.6f);
            BorderColor = Util.IsGtk ? SystemColors.WindowBackground : SystemColors.Control;
        }

        public static Font TextFont { get; set; }
        
        public static int TextHeight { get; set; }

        public static Color TextColor { get; set; }

        public static Color BackColor { get; set; }

        public static Color HoverTextColor { get; set; }

        public static Color HoverBackColor { get; set; }

        public static Color DisabledTextColor { get; set; }

        public static Color BorderColor { get; set; }

        public static void Update(Graphics g)
        {
            TextHeight = (int)(SystemFonts.Default().LineHeight * g.PixelsPerPoint + 0.5);
        }

        public static Color GetTextColor(bool selected, bool disabled)
        {
            if (disabled)
                return DisabledTextColor;

            return selected ? HoverTextColor : TextColor;
        }

        public static Color GetBackgroundColor(bool selected)
        {
            return selected ? HoverBackColor : BackColor;
        }
    }
}
