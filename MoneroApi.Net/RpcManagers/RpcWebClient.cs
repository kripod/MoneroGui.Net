using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Jojatekok.MoneroAPI.RpcManagers
{
    public sealed class RpcWebClient
    {
        public string Host { get; private set; }
        public ushort PortDaemon { get; private set; }
        public ushort PortWallet { get; private set; }

        private static readonly JsonSerializer JsonSerializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
        private static readonly Encoding Encoding = Encoding.UTF8;

        internal RpcWebClient(string host, ushort portDaemon, ushort portWallet)
        {
            Host = host;
            PortDaemon = portDaemon;
            PortWallet = portWallet;
        }

        public async Task<T> HttpGetDataAsync<T>(RpcPortType portType, string command)
        {
            var jsonString = await HttpQueryStringAsync(portType, "GET", command);
            var output = JsonSerializer.DeserializeObject<T>(jsonString);

            return output;
        }

        public async Task<T> HttpPostDataAsync<T>(RpcPortType portType, string command, string postData)
        {
            var jsonString = await PostStringAsync(portType, command, postData);
            var output = JsonSerializer.DeserializeObject<T>(jsonString);

            return output;
        }

        private async Task<string> HttpQueryStringAsync(RpcPortType portType, string method, string relativeUrl)
        {
            var request = CreateHttpWebRequest(portType, method, relativeUrl);
            return await request.GetResponseStringAsync();
        }

        public async Task<T> JsonQueryDataAsync<T>(RpcPortType portType, JsonRpcRequest jsonRpcRequest)
        {
            var jsonString = await PostStringAsync(portType, "json_rpc", JsonSerializer.SerializeObject(jsonRpcRequest));
            var output = JsonSerializer.DeserializeObject<JsonRpcResponse<T>>(jsonString);

            return output.Result;
        }

        public Task<T> JsonQueryDataAsync<T>(RpcPortType portType, string command)
        {
            return JsonQueryDataAsync<T>(portType, new JsonRpcRequest(command));
        }

        private HttpWebRequest CreateHttpWebRequest(RpcPortType portType, string method, string relativeUrl)
        {
            var request = WebRequest.CreateHttp(GetBaseUrl(portType) + relativeUrl);

            request.Method = method;
            request.Timeout = Helper.RequestsTimeoutMilliseconds;

            return request;
        }

        private async Task<string> PostStringAsync(RpcPortType portType, string relativeUrl, string postData)
        {
            var request = CreateHttpWebRequest(portType, "POST", relativeUrl);
            request.ContentType = "application/json";

            var postBytes = Encoding.GetBytes(postData);
            request.ContentLength = postBytes.Length;

            using (var requestStream = await request.GetRequestStreamAsync()) {
                await requestStream.WriteAsync(postBytes, 0, postBytes.Length);
            }

            return await request.GetResponseStringAsync();
        }

        private string GetBaseUrl(RpcPortType portType)
        {
            ushort port;
            switch (portType) {
                case RpcPortType.Wallet:
                    port = PortWallet;
                    break;

                default:
                    port = PortDaemon;
                    break;
            }

            return "http://" + Host + ":" + port + "/";
        }
    }
}
