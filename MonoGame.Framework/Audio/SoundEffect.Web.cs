// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using static Retyped.dom;
using static Retyped.es5;

namespace Microsoft.Xna.Framework.Audio
{
    public sealed partial class SoundEffect : IDisposable
    {
        // This platform is only limited by memory.
        internal const int MAX_PLAYING_INSTANCES = int.MaxValue;
        internal AudioBuffer _buffer;

        internal SoundEffect()
        {

        }

        internal void SetEmptyBuffer()
        {
            _buffer = SoundEffectInstance.Context.createBuffer(1, 1, 22050);
        }

        private void PlatformLoadAudioStream(Stream s, out TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        private void PlatformInitializePcm(byte[] buffer, int offset, int count, int sampleBits, int sampleRate, AudioChannels channels, int loopStart, int loopLength)
        {
            throw new NotImplementedException();
        }

        private void PlatformInitializeFormat(byte[] header, byte[] buffer, int bufferSize, int loopStart, int loopLength)
        {
            throw new NotImplementedException();
        }

        private void PlatformInitializeXact(MiniFormatTag codec, byte[] buffer, int channels, int sampleRate, int blockAlignment, int loopStart, int loopLength, out TimeSpan duration)
        {
            throw new NotSupportedException("Unsupported sound format!");
        }

        public static async Task<SoundEffect> FromURL(string url)
        {
            var ret = new SoundEffect();
            var loaded = false;
            var request = new XMLHttpRequest();

            request.open("GET", url, true);
            request.responseType = XMLHttpRequestResponseType.arraybuffer;

            request.onload += (a) => {
                SoundEffectInstance.Context.decodeAudioData(request.response.As<ArrayBuffer>(), (buffer) => {
                    ret._buffer = buffer;
                    loaded = true;
                });
            };
            request.send();

            while (!loaded)
                await Task.Delay(10);

            return ret;
        }

        private void PlatformSetupInstance(SoundEffectInstance instance)
        {
            instance.PlatformInitialize(_buffer);
        }

        private void PlatformDispose(bool disposing)
        {
        }

        internal static void PlatformSetReverbSettings(ReverbSettings reverbSettings)
        {
            
        }

        internal static void InitializeSoundEffect()
        {
        }

        internal static void PlatformShutdown()
        {

        }
    }
}

