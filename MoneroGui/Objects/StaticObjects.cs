using Jojatekok.MoneroAPI;

namespace Jojatekok.MoneroGUI
{
    static class StaticObjects
    {
        internal static MoneroClient MoneroClient { get; set; }

        internal static Logger LoggerDaemon { get; set; }
        internal static Logger LoggerWallet { get; set; }

        static StaticObjects()
        {
            MoneroClient = new MoneroClient();

            LoggerDaemon = new Logger();
            LoggerWallet = new Logger();
        }
    }
}
