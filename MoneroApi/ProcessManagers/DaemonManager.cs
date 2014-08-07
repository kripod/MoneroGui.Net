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
    public class DaemonManager : BaseProcessManager, IBaseProcessManager
    {
        public event EventHandler BlockchainSynced;
        public event EventHandler<NetworkInformationChangingEventArgs> NetworkInformationChanging;

        private static readonly string[] ProcessArgumentsDefault = { "--log-level 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        private Timer TimerCheckRpcAvailability { get; set; }
        private Timer TimerQueryNetworkInformation { get; set; }
        private Timer TimerSaveBlockchain { get; set; }

        private RpcWebClient RpcWebClient { get; set; }

        private bool _isRpcAvailable;
        public bool IsRpcAvailable {
            get { return _isRpcAvailable; }

            private set {
                _isRpcAvailable = value;
                if (!value) return;

                TimerQueryNetworkInformation.StartImmediately(TimerSettings.DaemonQueryNetworkInformationPeriod);
                if (IsBlockchainSavable) TimerSaveBlockchain.StartOnce(TimerSettings.DaemonSaveBlockchainPeriod);
            }
        }

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

        internal DaemonManager(RpcWebClient rpcWebClient, PathSettings paths) : base(paths.SoftwareDaemon)
        {
            Exited += Process_Exited;

            RpcWebClient = rpcWebClient;

            var rpcSettings = RpcWebClient.RpcSettings;

            ProcessArgumentsExtra = new List<string>(3) {
                "--data-dir \"" + paths.DirectoryDaemonData + "\"",
                "--rpc-bind-port " + rpcSettings.UrlPortDaemon
            };

            if (rpcSettings.UrlHost != StaticObjects.RpcUrlDefaultLocalhost) {
                ProcessArgumentsExtra.Add("--rpc-bind-ip " + rpcSettings.UrlHost);
            }

            TimerCheckRpcAvailability = new Timer(delegate { CheckRpcAvailability(); });
            TimerQueryNetworkInformation = new Timer(delegate { QueryNetworkInformation(); });
            TimerSaveBlockchain = new Timer(delegate { RequestSaveBlockchain(); });
        }

        public void Start()
        {
            StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra).ToArray());

            // Constantly check for the RPC port's activeness
            TimerCheckRpcAvailability.Change(TimerSettings.RpcCheckAvailabilityDueTime, TimerSettings.RpcCheckAvailabilityPeriod);
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

        private void CheckRpcAvailability()
        {
            if (Helper.IsPortInUse(RpcWebClient.RpcSettings.UrlPortDaemon)) {
                TimerCheckRpcAvailability.Stop();
                IsRpcAvailable = true;
            }
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

        private void Process_Exited(object sender, int e)
        {
            IsRpcAvailable = false;
            StopTimers();
        }

        private void StopTimers()
        {
            TimerCheckRpcAvailability.Stop();
            TimerQueryNetworkInformation.Stop();
            TimerSaveBlockchain.Stop();
        }

        private T HttpPostData<T>(string command) where T : RpcResponse
        {
            var output = RpcWebClient.HttpPostData<T>(RpcPortType.Daemon, command);
            if (output != null && output.Status == RpcResponseStatus.Ok) {
                return output;
            }

            return null;
        }

        private T JsonPostData<T>(JsonRpcRequest jsonRpcRequest) where T : RpcResponse
        {
            var output = RpcWebClient.JsonPostData<T>(RpcPortType.Daemon, jsonRpcRequest);
            if (output != null && output.Status == RpcResponseStatus.Ok) {
                return output;
            }

            return null;
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                TimerCheckRpcAvailability.Dispose();
                TimerCheckRpcAvailability = null;

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
