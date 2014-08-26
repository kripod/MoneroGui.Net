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

        public T HttpPostData<T>(ushort port, string command)
        {
            var jsonString = PostString(port, command);
            var output = JsonSerializer.DeserializeObject<T>(jsonString);

            return output;
        }

        public JsonRpcResponse<T> JsonPostData<T>(ushort port, JsonRpcRequest jsonRpcRequest)
        {
            var jsonString = PostString(port, "json_rpc", JsonSerializer.SerializeObject(jsonRpcRequest));
            return JsonSerializer.DeserializeObject<JsonRpcResponse<T>>(jsonString);
        }

        private string PostString(ushort port, string relativeUrl, string postData = null)
        {
            var request = WebRequest.CreateHttp(GetBaseUrl(port) + relativeUrl);
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

        private string GetBaseUrl(ushort port)
        {
            return "http://" + RpcSettings.UrlHost + ":" + port + "/";
        }
    }
}
