﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MonoGame.Content.Builder.Editor
{
    public static class Util
    {
        public static bool IsMac => true;

        public static bool IsLinux => false;

        public static bool IsWindows => false;

        public static bool IsGtk => false;

        public static bool IsXamarinMac => true;

        [DllImport("libc")]
        private static extern string realpath(string path, IntPtr resolved_path);

        public static string GetRealPath(string path)
        {
            // resolve symlinks on Unix systems
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return realpath(path, IntPtr.Zero);
            
            return path;
        }

        /// <summary>        
        /// Returns the path 'filspec' made relative path 'folder'.
        /// 
        /// If 'folder' is not an absolute path, throws ArgumentException.
        /// If 'filespec' is not an absolute path, returns 'filespec' unmodified.
        /// </summary>
        public static string GetRelativePath(string filespec, string folder)
        {
            if (!Path.IsPathRooted(filespec))
                return filespec;

            if (!Path.IsPathRooted(folder))
                throw new ArgumentException("Must be an absolute path.", "folder");

            filespec = Path.GetFullPath(filespec).TrimEnd(new[] { '/', '\\' });
            folder = Path.GetFullPath(folder).TrimEnd(new[] { '/', '\\' });

            if (filespec == folder)
                return string.Empty;

            var pathUri = new Uri(filespec);
            var folderUri = new Uri(folder + Path.DirectorySeparatorChar);
            var result = folderUri.MakeRelativeUri(pathUri).ToString();
            result = result.Replace('/', Path.DirectorySeparatorChar);
            result = Uri.UnescapeDataString(result);

            return result;
        }

        public static T Show<T>(this Eto.Forms.Dialog<T> dialog, Eto.Forms.Control parent)
        {
#if IDE
            return dialog.ShowModal(null);
#else
            return dialog.ShowModal(parent);
#endif
        }

        public static Eto.Forms.DialogResult Show(this Eto.Forms.CommonDialog dialog, Eto.Forms.Control parent)
        {
#if IDE
            return dialog.ShowDialog(null);
#else
            return dialog.ShowDialog(parent);
#endif
        }
    }
}
