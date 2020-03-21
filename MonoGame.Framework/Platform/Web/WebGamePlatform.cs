// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework.Graphics;
using WebAssembly;

namespace Microsoft.Xna.Framework
{
    class WebGamePlatform : GamePlatform
    {
        private WebGameWindow _view;
        private JSObject _window;
        private bool _exit;
        private Action<double> _loop;

        public WebGamePlatform(Game game)
            : base(game)
        {
            Window = _view = new WebGameWindow(game);

            _window = (JSObject)Runtime.GetGlobalObject("window");
            _loop = new Action<double>(AnimationFrame);
        }
        
        public override void Exit()
        {
            _exit = true;
        }

        public override void RunLoop()
        {
            throw new Exception("How did this happen?");
        }

        public override void StartRunLoop()
        {
            _window.Invoke("requestAnimationFrame", _loop);
        }

        public void AnimationFrame(double timestamp)
        {
            Game.Tick();

            if (!_exit)
                _window.Invoke("requestAnimationFrame", _loop);
        }

        public override bool BeforeUpdate(GameTime gameTime)
        {
            return true;
        }

        public override bool BeforeDraw(GameTime gameTime)
        {
            return true;
        }

        public override void EnterFullScreen()
        {

        }

        public override void ExitFullScreen()
        {

        }

        internal override void OnPresentationChanged(PresentationParameters pp)
        {
            BeginScreenDeviceChange(pp.IsFullScreen);
            EndScreenDeviceChange(string.Empty, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        public override void BeginScreenDeviceChange(bool willBeFullScreen)
        {
            _view.BeginScreenDeviceChange(willBeFullScreen);
        }

        public override void EndScreenDeviceChange(string screenDeviceName, int clientWidth, int clientHeight)
        {
            _view.EndScreenDeviceChange(screenDeviceName, clientWidth, clientHeight);
        }

        public override GameRunBehavior DefaultRunBehavior => GameRunBehavior.Asynchronous;
    }
}

