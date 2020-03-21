// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.Media
{
    public sealed partial class Song : IEquatable<Song>, IDisposable
    {
        private void PlatformInitialize(string fileName)
        {
            Console.WriteLine("Called: " + fileName);
        }

        internal void SetEventHandler(FinishedPlayingHandler handler) { }

        void PlatformDispose(bool disposing)
        {

        }

        internal void Play(TimeSpan? startPosition)
        {

        }

        internal void Resume()
        {

        }

        internal void Pause()
        {

        }

        internal void Stop()
        {

        }

        internal float Volume
        {
            get => 0f;
            set { }
        }

        public TimeSpan Position
        {
            get => TimeSpan.FromSeconds(0);
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
