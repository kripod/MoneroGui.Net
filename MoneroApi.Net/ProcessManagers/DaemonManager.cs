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
        public event EventHandler RpcInitialized;
        public event EventHandler BlockchainSynced;
        public event EventHandler<NetworkInformationChangingEventArgs> NetworkInformationChanging;

        private static readonly string[] ProcessArgumentsDefault = { "--log-level 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        private Timer TimerQueryNetworkInformation { get; set; }
        private Timer TimerSaveBlockchain { get; set; }

        private RpcWebClient RpcWebClient { get; set; }

        public bool IsRpcInitialized { get; private set; }
        public bool IsBlockchainSynced { get; private set; }

        public NetworkInformation NetworkInformation { get; private set; }

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
            TimerQueryNetworkInformation.Change(0, 750);
            TimerSaveBlockchain.Change(120000, 120000);
        }

        private void QueryNetworkInformation()
        {
            if (NetworkInformationChanging != null) {
                var output = RpcWebClient.HttpGetData<NetworkInformation>(RpcPortType.Daemon, RpcRelativeUrls.DaemonGetInformation);
                if (output.Status == RpcResponseStatus.Ok && output.BlockHeightTotal != 0) {
                    var blockHeaderValueContainer = RpcWebClient.JsonQueryData<BlockHeaderValueContainer>(RpcPortType.Daemon, new GetBlockHeaderByHeight(Math.Max(output.BlockHeightDownloaded - 1, 0)));
                    if (blockHeaderValueContainer != null && blockHeaderValueContainer.Status == RpcResponseStatus.Ok) {
                        output.BlockTimeCurrent = blockHeaderValueContainer.Value.Timestamp;

                        NetworkInformationChanging(this, new NetworkInformationChangingEventArgs(output));
                        NetworkInformation = output;

                        if (output.BlockHeightRemaining == 0 && !IsBlockchainSynced) {
                            IsBlockchainSynced = true;
                            if (BlockchainSynced != null) BlockchainSynced(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        private void SaveBlockchain()
        {
            RpcWebClient.HttpGetData<HttpRpcResponse>(RpcPortType.Daemon, RpcRelativeUrls.DaemonSaveBlockchain);
        }

        private void Process_OutputReceived(object sender, string e)
        {
            var dataLower = e.ToLower(Helper.InvariantCulture);

            if (dataLower.Contains("rpc server initialized") && !IsRpcInitialized) {
                StartRpcServices();

                IsRpcInitialized = true;
                if (RpcInitialized != null) RpcInitialized(this, EventArgs.Empty);
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

                base.Dispose();
            }
        }
    }
}
