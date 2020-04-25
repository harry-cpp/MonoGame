// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using static Retyped.dom;

namespace Microsoft.Xna.Framework.Media
{
    public sealed partial class Song : IEquatable<Song>, IDisposable
    {
        private SoundEffect _soundEffect;
        private SoundEffectInstance _soundEffectInstance;

        private void PlatformInitialize(string fileName)
        {
            Content.ContentManager.BlockContentLoaading = true;
            LoadSong(fileName.EndsWith(".mp3") ? fileName : fileName + ".mp3");

            // TODO: Fix sound looping and hook up to game loop to track song position
        }

        private async void LoadSong(string fileName)
        {
            _soundEffect = await SoundEffect.FromURL(fileName);
            _soundEffectInstance = _soundEffect.CreateInstance();
            Content.ContentManager.BlockContentLoaading = false;
        }
        
        internal void SetEventHandler(FinishedPlayingHandler handler) { }
		
        void PlatformDispose(bool disposing)
        {

        }

        internal void Play(TimeSpan? startPosition)
        {
            _soundEffectInstance.Play();
            _soundEffectInstance._source.loop = true;
        }

        internal void Resume()
        {
            _soundEffectInstance.Resume();
        }

        internal void Pause()
        {
            _soundEffectInstance.Pause();
        }

        internal void Stop()
        {
            _soundEffectInstance.Stop(true);
        }

        internal float Volume
        {
            get => (float)_soundEffectInstance.Volume;
            set => _soundEffectInstance.Volume = value;
        }

        public TimeSpan Position
        {
            get => TimeSpan.Zero;
        }

        private Album PlatformGetAlbum()
        {
            return null;
        }

        private Artist PlatformGetArtist()
        {
            return null;
        }

        private Genre PlatformGetGenre()
        {
            return null;
        }

        private TimeSpan PlatformGetDuration()
        {
            return _duration;
        }

        private bool PlatformIsProtected()
        {
            return false;
        }

        private bool PlatformIsRated()
        {
            return false;
        }

        private string PlatformGetName()
        {
            return _name;
        }

        private int PlatformGetPlayCount()
        {
            return _playCount;
        }

        private int PlatformGetRating()
        {
            return 0;
        }

        private int PlatformGetTrackNumber()
        {
            return 0;
        }
    }
}