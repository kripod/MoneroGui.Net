using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Http.Responses;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public class DaemonManager : BaseProcessManager
    {
        public event EventHandler BlockchainSynced;
        public event EventHandler<NetworkInformationChangingEventArgs> NetworkInformationChanging;

        private static readonly string[] ProcessArgumentsDefault = { "--log-level 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        private Timer TimerQueryNetworkInformation { get; set; }
        private Timer TimerSaveBlockchain { get; set; }

        private RpcWebClient RpcWebClient { get; set; }

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

        internal DaemonManager(RpcWebClient rpcWebClient, Paths paths) : base(paths.SoftwareDaemon)
        {
            OutputReceived += Process_OutputReceived;

            RpcWebClient = rpcWebClient;

            ProcessArgumentsExtra = new List<string>(2) {
                "--rpc-bind-ip " + RpcWebClient.Host,
                "--rpc-bind-port " + RpcWebClient.PortDaemon
            };

            TimerQueryNetworkInformation = new Timer(delegate { QueryNetworkInformation(); });
            TimerSaveBlockchain = new Timer(delegate { SaveBlockchain(); });
        }

        public void Start()
        {
            StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra).ToArray());
        }

        public void StartRpcServices()
        {
            TimerQueryNetworkInformation.StartImmediately(TimerSettings.DaemonQueryNetworkInformationPeriod);
            TimerSaveBlockchain.StartOnce(TimerSettings.DaemonSaveBlockchainPeriod);
        }

        private void QueryNetworkInformation()
        {
            TimerQueryNetworkInformation.Stop();

            var output = HttpGetData<NetworkInformation>(HttpRpcCommands.DaemonGetInformation);
            if (output != null && output.BlockHeightTotal != 0) {
                var blockHeaderLast = GetBlockHeaderLast();
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

        private void SaveBlockchain()
        {
            HttpGetData<RpcResponse>(HttpRpcCommands.DaemonSaveBlockchain);

            TimerSaveBlockchain.StartOnce(TimerSettings.DaemonSaveBlockchainPeriod);
        }

        public BlockHeader GetBlockHeaderLast()
        {
            var blockHeaderValueContainer = JsonQueryData<BlockHeaderValueContainer>(new GetBlockHeaderLast());
            if (blockHeaderValueContainer != null) {
                return blockHeaderValueContainer.Value;
            }

            return null;
        }

        private void Process_OutputReceived(object sender, string e)
        {
            var dataLower = e.ToLower(Helper.InvariantCulture);

            if (dataLower.Contains("rpc server initialized")) {
                StartRpcServices();
            }
        }

        private T HttpGetData<T>(string command) where T : RpcResponse
        {
            var output = RpcWebClient.HttpGetData<T>(RpcPortType.Daemon, command);
            if (output.Status == RpcResponseStatus.Ok) {
                return output;
            }

            return null;
        }

        private T JsonQueryData<T>(JsonRpcRequest jsonRpcRequest) where T : RpcResponse
        {
            var output = RpcWebClient.JsonQueryData<T>(RpcPortType.Daemon, jsonRpcRequest);
            if (output.Status == RpcResponseStatus.Ok) {
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
                base.Dispose();

                TimerQueryNetworkInformation.Dispose();
                TimerQueryNetworkInformation = null;

                TimerSaveBlockchain.Dispose();
                TimerSaveBlockchain = null;
            }
        }
    }
}
