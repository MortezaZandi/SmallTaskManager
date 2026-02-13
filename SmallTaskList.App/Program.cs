using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace SmallTaskList.App
{
    internal static class Program
    {
        private const int HostPort = 5050;
        private static readonly string HostUrl = $"http://localhost:{HostPort}";
        private static Process _webAppProcess;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Resolve CefSharp.Core.Runtime from x86/x64 subfolder when using AnyCPU (must be before any CefSharp type is used).
            AppDomain.CurrentDomain.AssemblyResolve += ResolveCefSharpCoreRuntime;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string webAppExePath = GetWebAppExePath();
            if (string.IsNullOrEmpty(webAppExePath))
            {
                MessageBox.Show(
                    "SmallTask not found. Build the SmallTask project first.",
                    "SmallTask",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                StartWebAppProcess(webAppExePath);
                WaitForWebAppReadyAsync().GetAwaiter().GetResult();

                var settings = new CefSettings();
                settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
                if (!Cef.IsInitialized.GetValueOrDefault() && !Cef.Initialize(settings))
                {
                    MessageBox.Show("CefSharp failed to initialize.", "SmallTask", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    StopWebApp();
                    return;
                }

                Application.Run(new Form1(HostUrl));
            }
            finally
            {
                StopWebApp();
                Cef.Shutdown();
            }
        }

        private static Assembly ResolveCefSharpCoreRuntime(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp.Core.Runtime,", StringComparison.OrdinalIgnoreCase))
            {
                string arch = Environment.Is64BitProcess ? "x64" : "x86";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, arch, "CefSharp.Core.Runtime.dll");
                if (File.Exists(path))
                    return Assembly.LoadFile(path);
            }
            return null;
        }

        internal static void StopWebApp()
        {
            if (_webAppProcess != null && !_webAppProcess.HasExited)
            {
                try { _webAppProcess.Kill(); } catch { }
                _webAppProcess = null;
            }
        }

        private static string GetWebAppExePath()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] relativePaths =
            {
                Path.Combine(baseDir, @"net7.0\SmallTask.exe"),
            };

            foreach (string relative in relativePaths)
            {
                string fullPath = Path.GetFullPath(relative);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }

        private static void StartWebAppProcess(string exePath)
        {
            string workingDir = Path.GetDirectoryName(exePath);
            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = workingDir,
                Arguments = $"--urls {HostUrl}",
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            startInfo.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = "Development";

            _webAppProcess = Process.Start(startInfo);
        }

        private static async Task WaitForWebAppReadyAsync()
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(2);
                int maxAttempts = 30;
                int delayMs = 200;

                for (int i = 0; i < maxAttempts; i++)
                {
                    try
                    {
                        var response = await client.GetAsync(HostUrl);
                        if (response.IsSuccessStatusCode)
                            return;
                    }
                    catch { }

                    await Task.Delay(delayMs);
                }
            }

            throw new InvalidOperationException($"Web app did not start within ~6 seconds. Check that {HostUrl} is not in use.");
        }
    }
}
