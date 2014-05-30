using System;
using System.Diagnostics;
using System.Timers;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public abstract class BaseProcessManager : IDisposable
    {
        protected event EventHandler<string> OutputReceived;
        protected event EventHandler<string> ErrorReceived;
        protected event EventHandler<int> Exited;

        private bool IsDisposeInProgress { get; set; }

        private Process Process { get; set; }
        private string Path { get; set; }

        protected bool IsProcessAlive {
            get { return Process != null && !Process.HasExited; }
        }

        protected BaseProcessManager(string path) {
            Path = path;
        }

        protected void StartProcess(params string[] arguments)
        {
            if (Process != null) Process.Dispose();

            Process = new Process {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(Path) {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true
                }
            };

            if (arguments != null) {
                Process.StartInfo.Arguments = string.Join(" ", arguments);
            }

            Process.Exited += Process_Exited;

            Process.Start();

            ReadLineAsync(true);
            ReadLineAsync(false);
        }

        private async void ReadLineAsync(bool isError)
        {
            while (IsProcessAlive) {
                var reader = isError ? Process.StandardError : Process.StandardOutput;
                var line = await reader.ReadLineAsync();
                if (IsDisposeInProgress) break;
                if (line == null) continue;

                if (isError) {
                    if (ErrorReceived != null) ErrorReceived(this, line);
                } else {
                    if (OutputReceived != null) OutputReceived(this, line);
                }
            }
        }

        protected void Send(string input)
        {
            if (IsProcessAlive) {
                Process.StandardInput.WriteLine(input);
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (Exited != null) Exited(this, Process.ExitCode);

            // TODO: Restart the process whether it's needed
            // StartProcess(Path);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !IsDisposeInProgress) {
                IsDisposeInProgress = true;

                if (Process != null) {
#if DEBUG // Unsafe shutdown
                    if (!Process.HasExited) {
                        Process.Kill();
                    }

#else     // Safe shutdown
                    if (!Process.HasExited) {
                        if (Process.Responding) {
                            Send("exit");
                            if (!Process.WaitForExit(30000)) Process.Kill();
                        } else {
                            Process.Kill();
                        }

                        Process.WaitForExit();
                    }
#endif

                    Process.Dispose();
                    Process = null;
                }
            }
        }
    }
}
