using Jojatekok.MoneroAPI.ProcessManagers;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Jojatekok.MoneroAPI
{
    static class Helper
    {
        // TODO: Add custom host and port support
        public const string RpcUrlBaseIp = "127.0.0.1";
        public const ushort RpcUrlBasePortDaemon = 18081;
        public const ushort RpcUrlBasePortWallet = 19091;

        public const string RpcUrlRelativeHttpGetInformation = "getinfo";
        public const string RpcUrlRelativeHttpGetTransactions = "gettransactions";
        public const string RpcUrlRelativeHttpGetSaveBlockchain = "save_bc";

        public const string RpcUrlRelativeHttpSendTransaction = "sendrawtransaction";

        public const int RequestsTimeoutMilliseconds = 30000;

        public static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static readonly JobManager JobManager = new JobManager();

        internal static DateTime UnixTimeStampToDateTime(ulong unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
        }

        public static async Task<string> GetResponseStringAsync(this HttpWebRequest request)
        {
            using (var response = await request.GetResponseAsync()) {
                using (var stream = response.GetResponseStream()) {
                    if (stream == null) throw new NullReferenceException("The HttpWebRequest's response stream is empty.");

                    using (var reader = new StreamReader(stream)) {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static T DeserializeObject<T>(this JsonSerializer serializer, string value)
        {
            using (var stringReader = new StringReader(value)) {
                using (var jsonTextReader = new JsonTextReader(stringReader)) {
                    return (T)serializer.Deserialize(jsonTextReader, typeof(T));
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static string SerializeObject<T>(this JsonSerializer serializer, T value)
        {
            using (var stringWriter = new StringWriter(InvariantCulture)) {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter)) {
                    serializer.Serialize(jsonTextWriter, value);
                }

                return stringWriter.ToString();
            }
        }
    }
}
