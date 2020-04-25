// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using Bridge;
using static Retyped.dom;
using static Retyped.es5;

namespace Microsoft.Xna.Framework.Audio
{
    public partial class SoundEffectInstance : IDisposable
    {
        internal static AudioContext Context = Script.Eval<AudioContext>("var AudioContext = window.AudioContext || window.webkitAudioContext || false; new AudioContext");

        private AudioBuffer _buffer;
        internal AudioBufferSourceNode _source;
        private PannerNode _pannerNode;
        private BiquadFilterNode _biquadFilterNode;
        private PannerNode _stereoPannerNode;
        private GainNode _gainNode;
        private SoundState _state;
        private double _endTime, _startTime;

        internal void PlatformInitialize(byte[] buffer, int sampleRate, int channels)
        {
            throw new NotImplementedException();
        }

        internal void PlatformInitialize(AudioBuffer buffer)
        {
            _buffer = buffer;
            _state = SoundState.Stopped;

            _stereoPannerNode = Context.createPanner();
            _pannerNode = Context.createPanner();
            _gainNode = Context.createGain();

            _stereoPannerNode.connect(_pannerNode);
            _pannerNode.connect(_gainNode);
            _gainNode.connect(Context.destination);
        }

        private void PlatformApply3D(AudioListener listener, AudioEmitter emitter)
        {
            // TODO: Read up the rules for this
            // _pannedNode.setPosition _pannedNode.setVelocity _pannedNode.setOrientation
        }

        private void PlatformPause()
        {
            if (_state != SoundState.Playing)
                return;

            _endTime = Context.currentTime;
            PlatformStop(true);
            _state = SoundState.Paused;
        }

        private void PlatformPlay()
        {
            if (_state == SoundState.Playing)
                return;

            _state = SoundState.Stopped;
            PlatformResume();
            _startTime = Context.currentTime;
        }

        private void PlatformResume()
        {
            if (_state == SoundState.Playing)
                return;

            var startPos = (_state == SoundState.Paused) ? _endTime - _startTime : 0;
            _startTime = Context.currentTime - startPos;

            // Re/create source, it needs to be recreated after each stop :\
            _source = Context.createBufferSource();
            _source.buffer = _buffer;
            _source.connect(_stereoPannerNode);

            _source.start(0, startPos);
            _state = SoundState.Playing;
        }

        private void PlatformStop(bool immediate)
        {
            if (_state != SoundState.Playing)
                return;

            _source.stop();
            _source.disconnect();

            _state = SoundState.Stopped;
        }

        private void PlatformSetIsLooped(bool value)
        {
            _source.loop = value;
        }

        private bool PlatformGetIsLooped()
        {
            return _source.loop;
        }

        private void PlatformSetPan(float value)
        {
            _stereoPannerNode.panningModel = PanningModelType.equalpower;
            _stereoPannerNode.setPosition(value, 0 , 1 - System.Math.Abs(value));
        }

        private void PlatformSetPitch(float value)
        {
            
        }

        private SoundState PlatformGetState()
        {
            return _state;
        }

        private void PlatformSetVolume(float value)
        {
            _gainNode.gain.value = value;
        }

        internal void PlatformSetReverbMix(float mix)
        {

        }

        internal void PlatformSetFilter(FilterMode mode, float filterQ, float frequency)
        {
            _biquadFilterNode.frequency.value = frequency;
            _biquadFilterNode.Q.value = filterQ;

            switch (mode)
            {
                case FilterMode.BandPass:
                    _biquadFilterNode.type = BiquadFilterType.bandpass;
                    break;
                case FilterMode.HighPass:
                    _biquadFilterNode.type = BiquadFilterType.highpass;
                    break;
                case FilterMode.LowPass:
                    _biquadFilterNode.type = BiquadFilterType.lowpass;
                    break;
            }

            //_stereoPannerNode.disconnect();
            _biquadFilterNode.disconnect();
            
            //_stereoPannerNode.connect(_biquadFilterNode);
            _biquadFilterNode.connect(_pannerNode);
        }

        internal void PlatformClearFilter()
        {
            _stereoPannerNode.disconnect();
            _biquadFilterNode.disconnect();

            _stereoPannerNode.connect(_pannerNode);
        }

        private void PlatformDispose(bool disposing)
        {
            PlatformStop(true);
        }
    }
}
