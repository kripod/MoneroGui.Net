using Jojatekok.MoneroAPI.RpcManagers;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Http.Responses;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Requests;
using Jojatekok.MoneroAPI.RpcManagers.Daemon.Json.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public class DaemonManager : BaseProcessManager
    {
        public event EventHandler RpcInitialized;
        public event EventHandler BlockchainSynced;
        public event EventHandler<NetworkInformationChangingEventArgs> NetworkInformationChanging;

        private static readonly string[] ProcessArgumentsDefault = { "--log-level 0" };
        private List<string> ProcessArgumentsExtra { get; set; }

        private RpcWebClient RpcWebClient { get; set; }

        public bool IsRpcInitialized { get; private set; }
        public bool IsBlockchainSynced { get; private set; }

        public NetworkInformation NetworkInformation { get; private set; }

        internal DaemonManager(RpcWebClient rpcWebClient, Paths paths) : base(paths.SoftwareDaemon)
        {
            ErrorReceived += Process_ErrorReceived;
            OutputReceived += Process_OutputReceived;

            RpcWebClient = rpcWebClient;

            ProcessArgumentsExtra = new List<string>(2) {
                "--rpc-bind-ip " + RpcWebClient.Host,
                "--rpc-bind-port " + RpcWebClient.PortDaemon
            };
        }

        public void Start()
        {
            StartProcess(ProcessArgumentsDefault.Concat(ProcessArgumentsExtra).ToArray());
        }

        public void StartRpcServices()
        {
            AutoQueryNetworkInformationAsync();
            AutoSaveBlockchainAsync();
        }

        private async void AutoQueryNetworkInformationAsync()
        {
            while (IsProcessAlive) {
                if (NetworkInformationChanging != null) {
                    var output = await RpcWebClient.HttpGetDataAsync<NetworkInformation>(RpcPortType.Daemon, Helper.RpcUrlRelativeHttpGetInformation);
                    if (output.Status == RpcResponseStatus.Ok && output.BlockHeightTotal != 0) {
                        var blockHeaderValueContainer = await RpcWebClient.JsonQueryDataAsync<BlockHeaderValueContainer>(RpcPortType.Daemon, new GetBlockHeaderByHeight(Math.Max(output.BlockHeightDownloaded - 1, 0)));
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

                await Task.Delay(750);
            }
        }

        private async void AutoSaveBlockchainAsync()
        {
            while (IsProcessAlive) {
                // TODO: Add support for custom intervals
                await Task.Delay(120000);
                await RpcWebClient.HttpGetDataAsync<HttpRpcResponse>(RpcPortType.Daemon, Helper.RpcUrlRelativeHttpGetSaveBlockchain);
            }
        }

        private void Process_ErrorReceived(object sender, string e)
        {
            // TODO: Handle daemon errors
        }

        private void Process_OutputReceived(object sender, string e)
        {
            var data = e;
            var dataLower = e.ToLower(Helper.InvariantCulture);

            if (dataLower.Contains("rpc server initialized") && !IsRpcInitialized) {
                StartRpcServices();

                IsRpcInitialized = true;
                if (RpcInitialized != null) RpcInitialized(this, EventArgs.Empty);
            }

            // Error handler
            if (dataLower.Contains("error")) Process_ErrorReceived(this, data);
        }
    }
}
