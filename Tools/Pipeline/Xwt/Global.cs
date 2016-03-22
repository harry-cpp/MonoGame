using System;
using System.Runtime.InteropServices;

namespace MonoGame.Tools.Pipeline
{
    public enum OS
    {
        Windows,
        Linux,
        MacOSX,
        Unknown
    }

    public static class Global
    {
        private static bool _init = false;
        private static OS _os;

        [DllImport ("libc")]
        static extern int uname (IntPtr buf);

        public static string WindowsNotAllowedCharacters = "<>:\"\\/|?*";
        public static string LinuxNotAllowedCharacters = "/"; 
        public static string MacNotAllowedCharacters = ":";

        public static OS OS
        {
            get
            {
                Init();
                return _os;
            }
        }

        public static string NotAllowedCharacters
        {
            get
            {
                Init();
                if (_os == OS.Windows)
                    return WindowsNotAllowedCharacters;
                else if (_os == OS.Linux)
                    return LinuxNotAllowedCharacters;
                else if (_os == OS.MacOSX)
                    return MacNotAllowedCharacters;

                return "";
            }
        }

        public static bool CheckString(string s, string notallowedCharacters)
        {
            for (int i = 0; i < notallowedCharacters.Length; i++) 
                if (s.Contains (notallowedCharacters.Substring (i, 1)))
                    return false;

            return true;
        }

        public static void Init()
        {
            if (!_init)
            {
                PlatformID pid = Environment.OSVersion.Platform;

                switch (pid) 
                {
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        _os = OS.Windows;
                        break;
                    case PlatformID.MacOSX:
                        _os = OS.MacOSX;
                        break;
                    case PlatformID.Unix:

                        // Mac can return a value of Unix sometimes, We need to double check it.
                        IntPtr buf = IntPtr.Zero;
                        try
                        {
                            buf = Marshal.AllocHGlobal (8192);

                            if (uname (buf) == 0) {
                                string sos = Marshal.PtrToStringAnsi (buf);
                                if (sos == "Darwin")
                                {
                                    _os = OS.MacOSX;
                                    return;
                                }
                            }
                        } catch {
                        } finally {
                            if (buf != IntPtr.Zero)
                                Marshal.FreeHGlobal (buf);
                        }

                        _os = OS.Linux;
                        break;
                    default:
                        _os = OS.Unknown;
                        break;
                }

                _init = true;
            }
        }
    }
}

