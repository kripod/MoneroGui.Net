using System;
using System.Diagnostics;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public abstract class BaseProcessManager : IDisposable
    {
        public event EventHandler<string> OnLogMessage;
        public event EventHandler<int> Exited;

        protected event EventHandler<string> OutputReceived;

        private Process Process { get; set; }
        private string Path { get; set; }

        private bool IsDisposing { get; set; }
        private bool IsProcessAlive {
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

            Process.OutputDataReceived += Process_OutputDataReceived;
            Process.Exited += Process_Exited;

            Process.Start();
            StaticObjects.JobManager.AddProcess(Process);
            Process.BeginOutputReadLine();
        }

        public void Send(string input)
        {
            if (IsProcessAlive) {
                if (OnLogMessage != null) OnLogMessage(this, "> " + input);
                Process.StandardInput.WriteLine(input);
            }
        }

        protected void KillBaseProcess()
        {
            if (IsProcessAlive) {
                Process.Kill();
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            var line = e.Data;
            if (line == null) return;

            if (OnLogMessage != null) OnLogMessage(this, line);
            if (OutputReceived != null) OutputReceived(this, line);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (IsDisposing) return;

            Process.CancelOutputRead();

            if (Exited != null) Exited(this, Process.ExitCode);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !IsDisposing) {
                IsDisposing = true;

                if (Process != null) {
                    if (!Process.HasExited) {
                        if (Process.Responding) {
                            if (!Process.WaitForExit(300000)) Process.Kill();
                        } else {
                            Process.Kill();
                        }
                    }

                    Process.Dispose();
                    Process = null;
                }
            }
        }
    }
}
