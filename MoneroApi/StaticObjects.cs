using Jojatekok.MoneroAPI.ProcessManagers;

namespace Jojatekok.MoneroAPI
{
    static class StaticObjects
    {
        public const string RpcUrlDefaultLocalhost = "localhost";

        public static readonly JobManager JobManager = new JobManager();
    }
}
