using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Http.Responses;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Responses;
using Jojatekok.MoneroAPI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public class DaemonManager : BaseRpcProcessManager
    {
        public event EventHandler BlockchainSynced;
        public event EventHandler<NetworkInformationChangingEventArgs> NetworkInformationChanging;

        private static readonly string[] ProcessArgumentsDefault = { "--log-level 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        private Timer TimerQueryNetworkInformation { get; set; }
        private Timer TimerSaveBlockchain { get; set; }

        private RpcWebClient RpcWebClient { get; set; }

        private bool _isBlockchainSavable;
        internal bool IsBlockchainSavable {
            get { return _isBlockchainSavable; }

            set {
                if (value == _isBlockchainSavable) return;
                _isBlockchainSavable = value;

                if (value && IsRpcAvailable) {
                    TimerSaveBlockchain.StartOnce(TimerSettings.DaemonSaveBlockchainPeriod);
                }
            }
        }

        private bool _isBlockchainSynced;
        public bool IsBlockchainSynced {
            get { return _isBlockchainSynced; }

            private set {
                _isBlockchainSynced = value;
                if (BlockchainSynced != null && value) BlockchainSynced(this, EventArgs.Empty);
            }
        }

        private NetworkInformation _networkInformation;
        public NetworkInformation NetworkInformation {
            get { return _networkInformation; }

            private set {
                if (NetworkInformationChanging != null) NetworkInformationChanging(this, new NetworkInformationChangingEventArgs(value));
                _networkInformation = value;
            }
        }

        internal DaemonManager(RpcWebClient rpcWebClient, PathSettings paths) : base(paths.SoftwareDaemon, rpcWebClient, rpcWebClient.RpcSettings.UrlPortDaemon)
        {
            RpcAvailabilityChanged += Process_RpcAvailabilityChanged;

            RpcWebClient = rpcWebClient;
            var rpcSettings = RpcWebClient.RpcSettings;

            ProcessArgumentsExtra = new List<string>(3) {
                //"--data-dir \"" + paths.DirectoryDaemonData + "\"",
                "--rpc-bind-port " + rpcSettings.UrlPortDaemon
            };

            if (rpcSettings.UrlHost != StaticObjects.RpcUrlDefaultLocalhost) {
                ProcessArgumentsExtra.Add("--rpc-bind-ip " + rpcSettings.UrlHost);
            }

            // TODO: Remove this temporary fix
            ProcessArgumentsExtra.Add("--data-dir \"" + paths.DirectoryDaemonData);

            TimerQueryNetworkInformation = new Timer(delegate { QueryNetworkInformation(); });
            TimerSaveBlockchain = new Timer(delegate { RequestSaveBlockchain(); });
        }

        public void Start()
        {
            StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra).ToArray());
        }

        public void Stop()
        {
            KillBaseProcess();
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        private void QueryNetworkInformation()
        {
            TimerQueryNetworkInformation.Stop();

            var output = HttpPostData<NetworkInformation>(HttpRpcCommands.DaemonGetInformation);
            if (output != null && output.BlockHeightTotal != 0) {
                var blockHeaderLast = QueryBlockHeaderLast();
                if (blockHeaderLast != null) {
                    output.BlockTimeCurrent = blockHeaderLast.Timestamp;

                    NetworkInformation = output;

                    if (output.BlockHeightRemaining == 0 && !IsBlockchainSynced) {
                        IsBlockchainSynced = true;
                    }
                }
            }

            TimerQueryNetworkInformation.StartOnce(TimerSettings.DaemonQueryNetworkInformationPeriod);
        }

        public BlockHeader QueryBlockHeaderLast()
        {
            var blockHeaderValueContainer = JsonPostData<BlockHeaderValueContainer>(new QueryBlockHeaderLast());
            if (blockHeaderValueContainer != null) {
                return blockHeaderValueContainer.Value;
            }

            return null;
        }

        private void RequestSaveBlockchain()
        {
            HttpPostData<RpcResponse>(HttpRpcCommands.DaemonSaveBlockchain);
        }

        private void Process_RpcAvailabilityChanged(object sender, EventArgs e)
        {
            if (IsRpcAvailable) {
                TimerQueryNetworkInformation.StartImmediately(TimerSettings.DaemonQueryNetworkInformationPeriod);
                if (IsBlockchainSavable) TimerSaveBlockchain.StartOnce(TimerSettings.DaemonSaveBlockchainPeriod);

            } else {
                TimerQueryNetworkInformation.Stop();
                TimerSaveBlockchain.Stop();
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
                TimerQueryNetworkInformation.Dispose();
                TimerQueryNetworkInformation = null;

                TimerSaveBlockchain.Dispose();
                TimerSaveBlockchain = null;

                // Safe shutdown
                if (IsRpcAvailable) {
                    HttpPostData<RpcResponse>(HttpRpcCommands.DaemonExit);
                }

                base.Dispose();
            }
        }
    }
}
