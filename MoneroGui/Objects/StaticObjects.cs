using Jojatekok.MoneroAPI;

namespace Jojatekok.MoneroGUI
{
    static class StaticObjects
    {
        public static MoneroClient MoneroClient { get; private set; }

        public static Logger LoggerDaemon { get; private set; }
        public static Logger LoggerWallet { get; private set; }

        static StaticObjects()
        {
            MoneroClient = new MoneroClient();

            LoggerDaemon = new Logger();
            LoggerWallet = new Logger();
        }
    }
}
