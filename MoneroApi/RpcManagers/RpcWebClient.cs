using Jojatekok.MoneroAPI.Settings;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading;

namespace Jojatekok.MoneroAPI.RpcManagers
{
    public sealed class RpcWebClient
    {
        public RpcSettings RpcSettings { get; private set; }

        private static readonly JsonSerializer JsonSerializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
        private static readonly Encoding EncodingUtf8 = Encoding.UTF8;

        internal RpcWebClient(RpcSettings rpcSettings)
        {
            RpcSettings = rpcSettings;
        }

        public T HttpPostData<T>(RpcPortType portType, string command)
        {
            var jsonString = PostString(portType, command, null);
            var output = JsonSerializer.DeserializeObject<T>(jsonString);

            return output;
        }

        public T JsonPostData<T>(RpcPortType portType, JsonRpcRequest jsonRpcRequest)
        {
            var jsonString = PostString(portType, "json_rpc", JsonSerializer.SerializeObject(jsonRpcRequest));
            var output = JsonSerializer.DeserializeObject<JsonRpcResponse<T>>(jsonString);

            return output.Result;
        }

        private string PostString(RpcPortType portType, string relativeUrl, string postData = null)
        {
            var request = WebRequest.CreateHttp(GetBaseUrl(portType) + relativeUrl);
            request.Method = "POST";
            request.Timeout = Timeout.Infinite;

            if (postData != null) {
                request.ContentType = "application/json";
                var postBytes = EncodingUtf8.GetBytes(postData);
                request.ContentLength = postBytes.Length;

                using (var requestStream = request.GetRequestStream()) {
                    requestStream.Write(postBytes, 0, postBytes.Length);
                }
            }

            return request.GetResponseString();
        }

        private string GetBaseUrl(RpcPortType portType)
        {
            ushort port;
            switch (portType) {
                case RpcPortType.Wallet:
                    port = RpcSettings.UrlPortWallet;
                    break;

                default:
                    port = RpcSettings.UrlPortDaemon;
                    break;
            }

            return "http://" + RpcSettings.UrlHost + ":" + port + "/";
        }
    }
}
