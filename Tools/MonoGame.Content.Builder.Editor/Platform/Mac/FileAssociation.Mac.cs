using MonoGame.Tools.Pipeline.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MonoGame.Tools.Pipeline
{
    public static class FileAssociation
    {
        private readonly static string appPath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Applications/MGCB Editor.app");
        private const string lsregisterPath = "/System/Library/Frameworks/CoreServices.framework/Frameworks/LaunchServices.framework/Support/lsregister";

        public static void Associate()
        {
            InstallApplication();
            InstallMimetype();
        }

        public static void Unassociate()
        {
            UninstallMimetype();
            UninstallApplication();
        }

        private static void InstallApplication()
        {
            Console.WriteLine("Installing application...");

            var baseAppPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "../.."));
            if (Path.GetExtension(baseAppPath) != ".app")
            {
                throw new FileNotFoundException("Not running from within the app package");
            }

            CopyDirectory(baseAppPath, appPath);

            Console.WriteLine("Installation complete!");
        }

        private static void InstallMimetype()
        {
            Console.WriteLine("Installing mimetype...");
            RunLsregister("-v -f");
            Console.WriteLine("Installation complete!");
        }

        private static void UninstallApplication()
        {
            Console.WriteLine("Uninstalling aplication...");

            if (Directory.Exists(appPath))
            {
                Directory.Delete(appPath, true);
            }

            Console.WriteLine("Uninstallation complete!");
        }

        private static void UninstallMimetype()
        {
            Console.WriteLine("Uninstalling mimetype...");
            RunLsregister("-v -f -u");
            Console.WriteLine("Uninstallation complete!");
        }

        private static void RunLsregister(string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = lsregisterPath,
                    Arguments = $"{arguments} \"{appPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.OutputDataReceived += (sender, eventArgs) => Console.WriteLine(eventArgs.Data);
            process.ErrorDataReceived += (sender, eventArgs) => Console.Error.WriteLine(eventArgs.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }

        private static void CopyDirectory(string sourceDirName, string destDirName)
        {
            var dir = new DirectoryInfo(sourceDirName);

            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            foreach (var file in dir.GetFiles())
                file.CopyTo(Path.Combine(destDirName, file.Name), false);

            foreach (var subdir in dir.GetDirectories())
                CopyDirectory(subdir.FullName, Path.Combine(destDirName, subdir.Name));
        }
    }
}
