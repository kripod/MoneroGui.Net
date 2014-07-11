using Jojatekok.MoneroAPI.ProcessManagers;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace Jojatekok.MoneroAPI
{
    static class Helper
    {
        public const string RpcUrlDefaultLocalhost = "localhost";

        // TODO: Add custom host and port support
        public const string RpcUrlIp = RpcUrlDefaultLocalhost;
        public const ushort RpcUrlPortDaemon = 18081;
        public const ushort RpcUrlPortWallet = 19091;

        public static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static readonly JobManager JobManager = new JobManager();

        public static string GetResponseString(this HttpWebRequest request)
        {
            using (var response = request.GetResponse()) {
                using (var stream = response.GetResponseStream()) {
                    Debug.Assert(stream != null, "stream != null");

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

        public static void StartImmediately(this Timer timer, int period)
        {
            if (timer == null) return;
            timer.Change(0, period);
        }

        public static void StartOnce(this Timer timer, int dueTime)
        {
            if (timer == null) return;
            timer.Change(dueTime, Timeout.Infinite);
        }

        public static void Stop(this Timer timer)
        {
            if (timer == null) return;
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static bool IsPortInUse(int port)
        {
            var activeTcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            for (var i = activeTcpListeners.Length - 1; i >= 0; i--) {
                if (activeTcpListeners[i].Port == port) {
                    return true;
                }
            }

            return false;
        }

        public static DateTime UnixTimeStampToDateTime(ulong unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
        }
    }
}
