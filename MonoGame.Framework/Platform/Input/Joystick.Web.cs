// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;

namespace Microsoft.Xna.Framework.Input
{
    static partial class Joystick
    {
        private const bool PlatformIsSupported = true;

        private static int PlatformLastConnectedIndex = 0;

        internal static bool TrackEvents = false;

        private static JoystickState PlatformGetState(int index)
        {
            throw new NotImplementedException();
        }

        private static JoystickCapabilities PlatformGetCapabilities(int index)
        {
            throw new NotImplementedException();
        }

        private static void PlatformGetState(ref JoystickState joystickState, int index)
        {
            throw new NotImplementedException();
        }
    }
}
