using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;

namespace MonoGame.Content.Builder.Editor.Property
{
    public class ColorDialog : Dialog
    {
        public ColorDialog()
        {
            XamlReader.Load(this, "MonoGame.Content.Builder.Editor.Property.Dialogs.ColorDialog.xeto");
        }
    }
}
