// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Retyped.dom;

namespace Microsoft.Xna.Framework
{
    class WebGamePlatform : GamePlatform
    {
        private WebGameWindow _view;
        private bool _exit;

        public WebGamePlatform(Game game)
            : base(game)
        {
            Window = _view = new WebGameWindow(game);
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
            // Safari workaround so sound effects would play
            // this means Run needs to be called on something like a button click
            var se = new SoundEffect();
            se.SetEmptyBuffer();
            se.Play();

            window.requestAnimationFrame(AnimationFrame);
        }

        public void AnimationFrame(double timestamp)
        {
            Game.Tick();

            if (!_exit)
                window.requestAnimationFrame(AnimationFrame);
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

