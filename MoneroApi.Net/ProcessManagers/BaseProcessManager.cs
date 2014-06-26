using System;
using System.Diagnostics;
using System.Threading;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public abstract class BaseProcessManager : IDisposable
    {
        public event EventHandler<string> OnLogMessage;
        public event EventHandler<int> Exited;

        protected event EventHandler<string> OutputReceived;
        protected event EventHandler<string> ErrorReceived;

        protected CancellationTokenSource TaskCancellation;

        private Process Process { get; set; }
        private string Path { get; set; }

        protected bool IsProcessAlive {
            get { return Process != null && !Process.HasExited; }
        }

        protected bool IsTaskCancellationRequested
        {
            get { return !IsProcessAlive || (TaskCancellation != null && TaskCancellation.IsCancellationRequested); }

            set {
                if (value) {
                    if (TaskCancellation == null) TaskCancellation = new CancellationTokenSource();
                    TaskCancellation.Cancel(false);

                } else {
                    if (TaskCancellation != null) TaskCancellation.Dispose();
                    TaskCancellation = new CancellationTokenSource();
                }
            }
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
            Helper.JobManager.AddProcess(Process);

            ReadLineAsync(true);
            ReadLineAsync(false);

            IsTaskCancellationRequested = false;
        }

        private async void ReadLineAsync(bool isError)
        {
            while (!IsTaskCancellationRequested) {
                var reader = isError ? Process.StandardError : Process.StandardOutput;
                var line = await reader.ReadLineAsync();
                if (line == null) continue;

                if (OnLogMessage != null) OnLogMessage(this, line);

                if (isError) {
                    if (ErrorReceived != null) ErrorReceived(this, line);
                } else {
                    if (OutputReceived != null) OutputReceived(this, line);
                }
            }
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
            IsTaskCancellationRequested = true;

            if (IsProcessAlive) Process.Kill();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (Exited != null) Exited(this, Process.ExitCode);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                IsTaskCancellationRequested = true;

                if (Process != null) {
#if DEBUG // Unsafe shutdown
                    KillBaseProcess();

#else     // Safe shutdown
                    if (!Process.HasExited) {
                        if (Process.Responding) {
                            Send("exit");
                            if (!Process.WaitForExit(120000)) Process.Kill();
                        } else {
                            Process.Kill();
                        }
                    }
#endif

                    Process.Dispose();
                    Process = null;
                }

                if (TaskCancellation != null) TaskCancellation.Dispose();
            }
        }
    }
}
