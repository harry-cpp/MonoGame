// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using WebAssembly;
using WebGLDotNET;

static class WebHelper
{
    public static WebGL2RenderingContext gl;
}

namespace Microsoft.Xna.Framework
{
    class WebGameWindow : GameWindow
    {
        private JSObject _canvas;

        public WebGameWindow(Game game)
        {
            using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            using (var body = (JSObject)document.GetObjectProperty("body"))
            {
                _canvas = (JSObject)document.Invoke("createElement", "canvas");
                _canvas.SetObjectProperty("width", 800);
                _canvas.SetObjectProperty("height", 480);
                body.Invoke("appendChild", _canvas);
            }

            WebHelper.gl = new WebGL2RenderingContext(_canvas);
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
        }

        protected internal override void SetSupportedOrientations(DisplayOrientation orientations)
        {
        }

        protected override void SetTitle(string title)
        {

        }

        public override bool AllowUserResizing
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public override Rectangle ClientBounds
        {
            get
            {
                return new Rectangle(0, 0, 800, 480);
            }
        }

        public override DisplayOrientation CurrentOrientation
        {
            get
            {
                return DisplayOrientation.Default;
            }
        }

        public override IntPtr Handle
        {
            get
            {
                return IntPtr.Zero;
            }
        }

        public override string ScreenDeviceName
        {
            get
            {
                return string.Empty;
            }
        }
    }
}

