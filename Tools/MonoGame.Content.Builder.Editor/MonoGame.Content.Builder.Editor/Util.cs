using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Eto.Forms;

namespace MonoGame.Content.Builder.Editor
{
    public static class Util
    {
        public static bool IsMac { get; set; }

        public static bool IsLinux { get; set; }

        public static bool IsWindows { get; set; }

        public static bool IsGtk { get; set; }

        public static bool IsXamarinMac { get; set; }

        public static bool IsWpf { get; set; }

        public static bool IsIde { get; set; }

        public static Window MainWindow { get; set; }

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

        public static bool CheckString(string s)
        {
            var notAllowed = Path.GetInvalidFileNameChars();

            for (int i = 0; i < notAllowed.Length; i++)
                if (s.Contains(notAllowed[i].ToString()))
                    return false;

            return true;
        }

        public static T Show<T>(this Dialog<T> dialog)
        {
            return dialog.ShowModal(MainWindow);
        }

        public static Task ShowAsync(this Dialog dialog)
        {
            return dialog.ShowModalAsync(MainWindow);
        }

        public static DialogResult Show(this CommonDialog dialog)
        {
            return dialog.ShowDialog(MainWindow);
        }
    }
}
