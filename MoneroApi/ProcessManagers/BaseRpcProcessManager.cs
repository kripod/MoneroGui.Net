using Jojatekok.MoneroAPI.RpcManagers;
using System;
using System.Diagnostics;
using System.Threading;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public abstract class BaseRpcProcessManager : IDisposable
    {
        public event EventHandler<string> OnLogMessage;

        protected event EventHandler RpcAvailabilityChanged;
        protected event EventHandler<string> OutputReceived;
        protected event EventHandler<int> Exited;

        private Process Process { get; set; }
        private string Path { get; set; }
        private RpcWebClient RpcWebClient { get; set; }
        private ushort RpcPort { get; set; }

        private Timer TimerCheckRpcAvailability { get; set; }

        private bool _isRpcAvailable;
        protected bool IsRpcAvailable {
            get { return _isRpcAvailable; }

            private set {
                if (value == _isRpcAvailable) return;

                _isRpcAvailable = value;
                if (value) TimerCheckRpcAvailability.Stop();
                if (RpcAvailabilityChanged != null) RpcAvailabilityChanged(this, EventArgs.Empty);
            }
        }

        private bool IsDisposing { get; set; }
        private bool IsProcessAlive {
            get { return Process != null && !Process.HasExited; }
        }

        protected BaseRpcProcessManager(string path, RpcWebClient rpcWebClient, ushort rpcPort) {
            Path = path;
            RpcWebClient = rpcWebClient;
            RpcPort = rpcPort;

            TimerCheckRpcAvailability = new Timer(delegate { CheckRpcAvailability(); });
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

            // Constantly check for the RPC port's activeness
            TimerCheckRpcAvailability.Change(TimerSettings.RpcCheckAvailabilityDueTime, TimerSettings.RpcCheckAvailabilityPeriod);
        }

        private void CheckRpcAvailability()
        {
            IsRpcAvailable = Helper.IsPortInUse(RpcPort);
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

        protected T HttpPostData<T>(string command) where T : RpcResponse
        {
            var output = RpcWebClient.HttpPostData<T>(RpcPort, command);
            if (output != null && output.Status == RpcResponseStatus.Ok) {
                return output;
            }

            return null;
        }

        protected T JsonPostData<T>(JsonRpcRequest jsonRpcRequest) where T : class
        {
            var output = RpcWebClient.JsonPostData<T>(RpcPort, jsonRpcRequest);
            var rpcResponse = output as RpcResponse;
            if (rpcResponse == null || rpcResponse.Status == RpcResponseStatus.Ok) {
                return output;
            }

            return null;
        }

        protected void JsonPostData(JsonRpcRequest jsonRpcRequest)
        {
            JsonPostData<object>(jsonRpcRequest);
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

            IsRpcAvailable = false;
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

                TimerCheckRpcAvailability.Dispose();
                TimerCheckRpcAvailability = null;

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
