using System;
using System.Text.RegularExpressions;
using System.Timers;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public class DaemonManager : BaseProcessManager, IDisposable
    {
        public event EventHandler<SyncStatusChangedEventArgs> SyncStatusChanged;
        public event EventHandler<byte> ConnectionCountChanged;

        public byte ConnectionCount { get; private set; }

        private Timer PingTimer { get; set; }
        private Timer ConnectionCountQueryTimer { get; set; }

        internal DaemonManager(Paths paths) : base(paths.SoftwareDaemon)
        {
            ErrorReceived += Process_ErrorReceived;
            OutputReceived += Process_OutputReceived;

            StartProcess();

            PingTimer = new Timer(1000);
            PingTimer.Elapsed += ((sender, e) => Send(""));
            PingTimer.Start();

            ConnectionCountQueryTimer = new Timer(5000);
            ConnectionCountQueryTimer.Elapsed += ((sender, e) => Send("print_cn"));
            ConnectionCountQueryTimer.Start();
        }

        private void Process_ErrorReceived(object sender, string e)
        {
            // TODO: Handle daemon errors
        }

        private void Process_OutputReceived(object sender, string e)
        {
            var data = e.ToLower(Helper.InvariantCulture);

            if (SyncStatusChanged != null && data.Contains("sync data return")) {
                var match = Regex.Match(data, "([0-9]+)->([0-9]+)\\[([0-9]+) blocks\\(([0-9]+) ([a-z]+)\\)");
                if (match.Success) {
                    SyncStatusChanged(this, new SyncStatusChangedEventArgs(
                        ulong.Parse(match.Groups[1].Value, Helper.InvariantCulture),
                        ulong.Parse(match.Groups[2].Value, Helper.InvariantCulture),
                        ulong.Parse(match.Groups[3].Value, Helper.InvariantCulture),
                        ulong.Parse(match.Groups[4].Value, Helper.InvariantCulture),
                        match.Groups[5].Value
                    ));
                }

                return;
            }

            if (ConnectionCountChanged != null) {
                if (Regex.IsMatch(data, "\\[out\\][0-9\\.:]+[\\s]+[0-9a-z]+")) {
                    ConnectionCount++;
                    ConnectionCountChanged(this, ConnectionCount);
                    return;
                }

                if (Regex.IsMatch(data, "remote host[\\s]+peer id")) {
                    ConnectionCount = 0;
                    ConnectionCountChanged(this, ConnectionCount);
                    return;
                }
            }
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                if (PingTimer != null) {
                    PingTimer.Dispose();
                    PingTimer = null;
                }

                if (ConnectionCountQueryTimer != null) {
                    ConnectionCountQueryTimer.Dispose();
                    ConnectionCountQueryTimer = null;
                }

                base.Dispose();
            }
        }
    }
}
