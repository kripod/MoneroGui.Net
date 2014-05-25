using System;
using System.Diagnostics;
using System.Timers;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public abstract class BaseProcessManager : IDisposable
    {
        protected EventHandler<string> OutputReceived;
        protected EventHandler<string> ErrorReceived;

        private bool IsDisposeInProgress { get; set; }

        private Process Process { get; set; }
        private string Path { get; set; }

        private Timer PingTimer { get; set; }

        protected bool IsProcessAlive {
            get { return Process != null && !Process.HasExited; }
        }

        protected BaseProcessManager(string path) {
            Path = path;

            PingTimer = new Timer(1000);
            PingTimer.Elapsed += ((sender, e) => Send(""));
        }

        protected void StartProcess()
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

            Process.Exited += Process_Exited;

            Process.Start();

            ReadLineAsync(true);
            ReadLineAsync(false);

            PingTimer.Start();
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

                if (PingTimer != null) {
                    PingTimer.Dispose();
                    PingTimer = null;
                }

                if (Process != null) {
                    if (Process.Responding) {
                        Send("exit");
                        if (!Process.WaitForExit(30000)) Process.Kill();
                    } else {
                        Process.Kill();
                    }

                    Process.WaitForExit();
                    Process.Dispose();
                    Process = null;
                }
            }
        }
    }
}
