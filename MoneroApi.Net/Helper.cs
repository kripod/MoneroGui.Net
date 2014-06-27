using Jojatekok.MoneroAPI.ProcessManagers;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;

namespace Jojatekok.MoneroAPI
{
    static class Helper
    {
        // TODO: Add custom host and port support
        public const string RpcUrlBaseIp = "127.0.0.1";
        public const ushort RpcUrlBasePortDaemon = 18081;
        public const ushort RpcUrlBasePortWallet = 19091;

        public const int RequestsTimeoutMilliseconds = 30000;

        public static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static readonly JobManager JobManager = new JobManager();

        public static string GetResponseString(this HttpWebRequest request)
        {
            using (var response = request.GetResponse()) {
                using (var stream = response.GetResponseStream()) {
                    if (stream == null) throw new NullReferenceException("The HttpWebRequest's response stream is empty.");

                    using (var reader = new StreamReader(stream)) {
                        return reader.ReadToEnd();
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

        public static void Stop(this Timer timer)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static DateTime UnixTimeStampToDateTime(ulong unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
        }
    }
}
