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
            // TODO: Implement custom wallet password support
            MoneroClient = new MoneroClient("x");

            LoggerDaemon = new Logger();
            LoggerWallet = new Logger();
        }
    }
}
