using System;
using System.Text.RegularExpressions;

namespace MoneroClient.ProcessManagers
{
    public class DaemonManager : BaseProcessManager
    {
        public EventHandler<SyncStatusChangedEventArgs> SyncStatusChanged;
        public EventHandler<byte> ConnectionCountChanged;

        public byte ConnectionCount { get; private set; }

        public DaemonManager() : base(Paths.ResourceDaemon)
        {
            ErrorReceived += Process_ErrorReceived;
            OutputReceived += Process_OutputReceived;
            
            StartProcess();
        }

        private void Process_ErrorReceived(object sender, string e)
        {
            // TODO: Handle daemon errors
        }

        private void Process_OutputReceived(object sender, string e)
        {
            var data = e.ToLower(Helper.InvariantCulture);

            if (SyncStatusChanged != null && data.Contains("sync data return")) {
                var match = Regex.Match(data, "([0-9]+)->([0-9]+)\\[([0-9]+) blocks\\(([0-9 a-z]+)\\)");
                if (match.Success) {
                    SyncStatusChanged(this, new SyncStatusChangedEventArgs(
                        ulong.Parse(match.Groups[1].Value, Helper.InvariantCulture),
                        ulong.Parse(match.Groups[2].Value, Helper.InvariantCulture),
                        string.Format(Helper.InvariantCulture, "{0} blocks ({1}) behind", match.Groups[3].Value, match.Groups[4].Value)
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
    }
}
