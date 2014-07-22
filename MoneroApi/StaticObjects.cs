using Jojatekok.MoneroAPI.ProcessManagers;
using System;

namespace Jojatekok.MoneroAPI
{
    static class StaticObjects
    {
        public const string RpcUrlDefaultLocalhost = "localhost";

        public static readonly string ApplicationDirectory = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly JobManager JobManager = new JobManager();
    }
}
