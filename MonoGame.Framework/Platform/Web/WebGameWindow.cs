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
            /*using (var canvasStyle = (JSObject)_canvas.GetObjectProperty("style"))
            {
                canvasStyle.SetObjectProperty("userSelect", "none");
                canvasStyle.SetObjectProperty("webkitUserSelect", "none");
                canvasStyle.SetObjectProperty("msUserSelect", "none");
            }*/

            WebHelper.gl = new WebGLRenderingContext(_canvas);

            // Block context menu on the canvas element
            //_canvas.oncontextmenu += (e) => e.preventDefault();

            // Connect events
            //_canvas.onmousemove += (e) => Canvas_MouseMove(e);
            //_canvas.onmousedown += (e) => Canvas_MouseDown(e);
            //_canvas.onmouseup += (e) => Canvas_MouseUp(e);
            //_canvas.onmousewheel += (e) => Canvas_MouseWheel(e);
            //_canvas.onkeydown += (e) => Canvas_KeyDown(e);
            //_canvas.onkeyup += (e) => Canvas_KeyUp(e);

            var _keyDown = new Func<JSObject, bool>(Canvas_KeyDown);
            document.Invoke("addEventListener", "onkeydown", _keyDown, false);
            //Console.WriteLine("called down");

            //_keyUp = new Func<JSObject, bool>(Canvas_KeyUp);
            //window.Invoke("onkeyup", _keyUp);

            //document.addEventListener("webkitfullscreenchange", Document_FullscreenChange);
            //document.addEventListener("mozfullscreenchange", Document_FullscreenChange);
            //document.addEventListener("fullscreenchange", Document_FullscreenChange);
            //document.addEventListener("MSFullscreenChange", Document_FullscreenChange);
        }

        private bool Canvas_KeyDown(JSObject e)
        {
            Console.WriteLine("called down");

            /*e.preventDefault();

            var xnaKey = KeyboardUtil.ToXna((int)e.keyCode, (int)e.location);

            if (!_keys.Contains(xnaKey))
                _keys.Add(xnaKey);

            Keyboard.CapsLock = ((int)e.keyCode == 20) ? !Keyboard.CapsLock : e.getModifierState("CapsLock");
            Keyboard.NumLock = ((int)e.keyCode == 144) ? !Keyboard.NumLock : e.getModifierState("NumLock");

            EnsureFullscreen();*/
            return true;
        }

        private bool Canvas_KeyUp(JSObject e)
        {
            Console.WriteLine("called up");

            /*_keys.Remove(KeyboardUtil.ToXna((int)e.keyCode, int.Parse(e.location.ToString())));

            EnsureFullscreen();*/
            return true;
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

