// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Bridge;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using static Retyped.dom;

static class WebHelper
{
    public static WebGLRenderingContext gl;
}

namespace Microsoft.Xna.Framework
{
    class WebGameWindow : GameWindow
    {
        private HTMLCanvasElement _canvas;
        private bool _isFullscreen, _willBeFullScreen;
        private string _screenDeviceName;
        private Game _game;
        private List<Keys> _keys;

        public WebGameWindow(Game game)
        {
            _game = game;
            _keys = new List<Keys>();

            Keyboard.SetKeys(_keys);

            _canvas = document.getElementById("monogamecanvas").As<HTMLCanvasElement>();
            _canvas.tabIndex = 1000;
            _canvas.focus();

            // Disable selection
            _canvas.style.userSelect = "none";
            _canvas.style.webkitUserSelect = "none";
            _canvas.style.msUserSelect = "none";

            // TODO: Move "GL context" creation outside the game window
            var possiblecontexts = new[] { "webgl", "experimental-webgl", "webkit-3d", "moz-webgl" };
            foreach(var context in possiblecontexts)
            {
                try
                {
                    WebHelper.gl = _canvas.getContext(context).As<WebGLRenderingContext>();
                    if (WebHelper.gl != null)
                        break;
                }
                catch { }
            }

            if (WebHelper.gl == null)
            {
                var d2d = _canvas.getContext("2d").As<CanvasRenderingContext2D>();
                d2d.fillStyle = "#6495ED";
                d2d.fillRect(0, 0, _canvas.width, _canvas.height);
                d2d.fillStyle = "#000000";
                d2d.font = "30px Arial";
                d2d.textAlign = "center";
                d2d.fillText("This device does not support WebGL  :(", _canvas.width / 2, _canvas.height / 2);

                throw new Exception("Failed to get WebGL context :|");
            }

            // Block context menu on the canvas element
            _canvas.oncontextmenu += (e) => e.preventDefault();

            // Connect events
            _canvas.onmousemove += (e) => Canvas_MouseMove(e);
            _canvas.onmousedown += (e) => Canvas_MouseDown(e);
            _canvas.onmouseup += (e) => Canvas_MouseUp(e);
            _canvas.onmousewheel += (e) => Canvas_MouseWheel(e);
            _canvas.onkeydown += (e) => Canvas_KeyDown(e);
            _canvas.onkeyup += (e) => Canvas_KeyUp(e);
            _canvas.ontouchstart += (e) => Canvas_TouchStart(e);
            _canvas.ontouchmove += (e) => Canvas_TouchMove(e);
            _canvas.ontouchend += (e) => Canvas_TouchEnd(e);
            _canvas.ontouchcancel += (e) => Canvas_TouchEnd(e);

            document.addEventListener("webkitfullscreenchange", Document_FullscreenChange);
            document.addEventListener("mozfullscreenchange", Document_FullscreenChange);
            document.addEventListener("fullscreenchange", Document_FullscreenChange);
            document.addEventListener("MSFullscreenChange", Document_FullscreenChange);
        }

        // Fullscreen can only be interacted with on user interaction events
        // so make sure we connect this code to as many input events as possible
        private void EnsureFullscreen()
        {
            /*var isfull = Script.Eval<bool>("(document.fullScreenElement && document.fullScreenElement !== null) || document.mozFullScreen || document.webkitIsFullScreen");

            if (_isFullscreen != isfull)
            {
                if (_isFullscreen)
                {
                    Script.Write(@"
                    var f_elem = document.getElementById('monogamecanvas');
                    var f_method = f_elem.requestFullscreen || f_elem.msRequestFullscreen || f_elem.mozRequestFullScreen || f_elem.webkitRequestFullscreen;
                    f_method.call(f_elem);
                    ");
                }
                else
                {
                    Script.Write(@"
                    var f_method = document.exitFullscreen || document.msExitFullscreen || document.mozCancelFullScreen || document.webkitExitFullscreen;
                    f_method.call(document);
                    ");
                }
            }*/
        }

        private void Document_FullscreenChange(Event e)
        {
            /*_isFullscreen = Script.Eval<bool>("(document.fullScreenElement && document.fullScreenElement !== null) || document.mozFullScreen || document.webkitIsFullScreen");
            
            if (_isFullscreen)
            {
                _canvas.width = (uint)document.documentElement.clientWidth;
                _canvas.height = (uint)document.documentElement.clientHeight;
            }
            else
            {
                _canvas.width = (uint)GraphicsDeviceManager.DefaultBackBufferWidth;
                _canvas.height = (uint)GraphicsDeviceManager.DefaultBackBufferHeight;
            }

            _game.GraphicsDevice.PresentationParameters.BackBufferWidth = (int)_canvas.width;
            _game.GraphicsDevice.PresentationParameters.BackBufferHeight = (int)_canvas.height;
            _game.GraphicsDevice.Viewport = new Graphics.Viewport(0, 0, (int)_canvas.width, (int)_canvas.height);

            _game.graphicsDeviceManager.IsFullScreen = _isFullscreen;*/
        }

        private void Canvas_TouchStart(TouchEvent e)
        {
            foreach (var touch in e.changedTouches)
            {
                var x = (int)(touch.clientX - _canvas.offsetLeft);
                var y = (int)(touch.clientY - _canvas.offsetTop);

                TouchPanelState.AddEvent(touch.identifier.As<int>(), TouchLocationState.Pressed, new Vector2(x, y));
            }
        }

        private void Canvas_TouchMove(TouchEvent e)
        {
            foreach (var touch in e.changedTouches)
            {
                var x = (int)(touch.clientX - _canvas.offsetLeft);
                var y = (int)(touch.clientY - _canvas.offsetTop);

                TouchPanelState.AddEvent(touch.identifier.As<int>(), TouchLocationState.Moved, new Vector2(x, y));
            }
        }

        private void Canvas_TouchEnd(TouchEvent e)
        {
            foreach (var touch in e.changedTouches)
            {
                var x = (int)(touch.clientX - _canvas.offsetLeft);
                var y = (int)(touch.clientY - _canvas.offsetTop);

                TouchPanelState.AddEvent(touch.identifier.As<int>(), TouchLocationState.Released, new Vector2(x, y));
            }
        }

        private bool Canvas_MouseMove(MouseEvent e)
        {
            this.MouseState.X = (int)(e.clientX - _canvas.offsetLeft);
            this.MouseState.Y = (int)(e.clientY - _canvas.offsetTop);

            return true;
        }

        private bool Canvas_MouseDown(MouseEvent e)
        {
            switch(e.button)
            {
                case 0:
                    this.MouseState.LeftButton = ButtonState.Pressed;
                    break;
                case 1:
                    this.MouseState.MiddleButton = ButtonState.Pressed;
                    break;
                case 2:
                    this.MouseState.RightButton = ButtonState.Pressed;
                    break;
            }

            EnsureFullscreen();
            return true;
        }

        private bool Canvas_MouseUp(MouseEvent e)
        {
            switch(e.button)
            {
                case 0:
                    this.MouseState.LeftButton = ButtonState.Released;
                    break;
                case 1:
                    this.MouseState.MiddleButton = ButtonState.Released;
                    break;
                case 2:
                    this.MouseState.RightButton = ButtonState.Released;
                    break;
            }

            EnsureFullscreen();
            return true;
        }

        private bool Canvas_MouseWheel(MouseEvent e)
        {
            if (e.detail < 0)
                this.MouseState.ScrollWheelValue += 120;
            else
                this.MouseState.ScrollWheelValue -= 120;

            return true;
        }

        private bool Canvas_KeyDown(KeyboardEvent e)
        {
            e.preventDefault();

            var xnaKey = KeyboardUtil.ToXna((int)e.keyCode, (int)e.location);

            if (!_keys.Contains(xnaKey))
                _keys.Add(xnaKey);

            Keyboard.CapsLock = ((int)e.keyCode == 20) ? !Keyboard.CapsLock : e.getModifierState("CapsLock");
            Keyboard.NumLock = ((int)e.keyCode == 144) ? !Keyboard.NumLock : e.getModifierState("NumLock");

            EnsureFullscreen();
            return true;
        }

        private bool Canvas_KeyUp(KeyboardEvent e)
        {
            _keys.Remove(KeyboardUtil.ToXna((int)e.keyCode, int.Parse(e.location.ToString())));

            EnsureFullscreen();
            return true;
        }

        public override bool AllowUserResizing
        {
            get => false;
            set { }
        }

        public override Rectangle ClientBounds => new Rectangle(0, 0, (int)_canvas.width, (int)_canvas.height);

        public override DisplayOrientation CurrentOrientation => DisplayOrientation.Default;

        public override IntPtr Handle => IntPtr.Zero;

        public override string ScreenDeviceName => _screenDeviceName;

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            _willBeFullScreen = willBeFullScreen;
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            _screenDeviceName = screenDeviceName;

            if (!_isFullscreen && !_willBeFullScreen)
            {
                _canvas.width = (uint)clientWidth;
                _canvas.height = (uint)clientHeight;
            }

            _isFullscreen = _willBeFullScreen;
        }

        protected override void SetTitle(string title)
        {
            document.title = title;
        }

        protected internal override void SetSupportedOrientations(DisplayOrientation orientations)
        {
            
        }
    }
}

