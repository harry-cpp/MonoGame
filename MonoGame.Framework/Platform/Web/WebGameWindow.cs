// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using WebAssembly;
using WebGLDotNET;
using static WebHelper;

static class WebHelper
{
    public static WebGLRenderingContext gl;

    public static JSObject document;

    public static JSObject body;

    public static JSObject window;
}

namespace Microsoft.Xna.Framework
{
    class WebGameWindow : GameWindow
    {
        private JSObject _canvas;
        private Game _game;
        private List<Keys> _keys;
        private string _screenDeviceName;
        //private Func<JSObject, bool> _keyDown, _keyUp;

        public WebGameWindow(Game game)
        {
            _game = game;
            _keys = new List<Keys>();

            Keyboard.SetKeys(_keys);

            WebHelper.document = (JSObject)Runtime.GetGlobalObject("document");
            WebHelper.window = (JSObject)Runtime.GetGlobalObject("window");
            WebHelper.body = (JSObject)document.GetObjectProperty("body");

            _canvas = (JSObject)document.Invoke("createElement", "canvas");
            _canvas.SetObjectProperty("tabIndex", 1000);
            _canvas.SetObjectProperty("width", 800);
            _canvas.SetObjectProperty("height", 480);
            body.Invoke("appendChild", _canvas);

            // Disable selection
            using (var canvasStyle = (JSObject)_canvas.GetObjectProperty("style"))
            {
                canvasStyle.SetObjectProperty("userSelect", "none");
                canvasStyle.SetObjectProperty("webkitUserSelect", "none");
                canvasStyle.SetObjectProperty("msUserSelect", "none");
            }

            WebHelper.gl = new WebGLRenderingContext(_canvas);

            // Block context menu on the canvas element
            _canvas.Invoke("addEventListener", "contextmenu", new Action<JSObject>((o) => {
                o.Invoke("preventDefault");
            }), false);

            _canvas.Invoke("addEventListener", "keydown", (Action<JSObject>)Canvas_KeyDown, false);
            _canvas.Invoke("addEventListener", "keyup", (Action<JSObject>)Canvas_KeyUp, false);
        }

        private void Canvas_KeyDown(JSObject e)
        {
            var keyCode = (int)e.GetObjectProperty("keyCode");
            var location = (int)e.GetObjectProperty("location");
            var xnaKey = KeyboardUtil.ToXna(keyCode, location);

            if (!_keys.Contains(xnaKey))
                _keys.Add(xnaKey);

            e.Invoke("preventDefault");
        }

        private void Canvas_KeyUp(JSObject e)
        {
            var keyCode = (int)e.GetObjectProperty("keyCode");
            var location = (int)e.GetObjectProperty("location");
            var xnaKey = KeyboardUtil.ToXna(keyCode, location);

            _keys.Remove(xnaKey);
        }

        public override bool AllowUserResizing
        {
            get => false;
            set { }
        }

        public override Rectangle ClientBounds
        {
            get => new Rectangle(0, 0, (int)_canvas.GetObjectProperty("width"), (int)_canvas.GetObjectProperty("height"));
        }

        public override DisplayOrientation CurrentOrientation => DisplayOrientation.Default;

        public override IntPtr Handle => IntPtr.Zero;

        public override string ScreenDeviceName => _screenDeviceName;

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            _screenDeviceName = screenDeviceName;
            _canvas.SetObjectProperty("width", clientWidth);
            _canvas.SetObjectProperty("height", clientHeight);
        }

        protected internal override void SetSupportedOrientations(DisplayOrientation orientations)
        {
        }

        protected override void SetTitle(string title)
        {

        }
    }
}

