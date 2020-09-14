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

        public static bool CheckString(string s)
        {
            var notAllowed = Path.GetInvalidFileNameChars();

            for (int i = 0; i < notAllowed.Length; i++)
                if (s.Contains(notAllowed[i].ToString()))
                    return false;

            return true;
        }

        public static string GetRelativePath(string relativeTo, string path)
        {
            var dirUri = new Uri(relativeTo.TrimEnd('\\', '/') + "/");
            var fileUri = new Uri(path);
            var relativeUri = dirUri.MakeRelativeUri(fileUri);

            if (relativeUri == null)
                return path;

            return Uri.UnescapeDataString(relativeUri.ToString());
        }

        public static T Show<T>(this Dialog<T> dialog)
        {
            return dialog.ShowModal(MainWindow);
        }

        public static Task ShowAsync(this Dialog dialog)
        {
            return dialog.ShowModalAsync(MainWindow);
        }

        public static Task<DialogResult> ShowAsync(this Dialog<DialogResult> dialog)
        {
            return dialog.ShowModalAsync(MainWindow);
        }

        public static DialogResult Show(this Dialog<DialogResult> dialog)
        {
            return dialog.ShowModal(MainWindow);
        }

        public static DialogResult Show(this CommonDialog dialog)
        {
            return dialog.ShowDialog(MainWindow);
        }
    }
}
