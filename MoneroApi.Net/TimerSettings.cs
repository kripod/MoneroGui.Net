namespace Jojatekok.MoneroAPI
{
    static class TimerSettings
    {
        public const int DaemonQueryNetworkInformationPeriod = 750;
        public const int DaemonSaveBlockchainPeriod = 300000;

        public const int WalletCheckRpcAvailabilityPeriod = 1000;
        public const int WalletCheckRpcAvailabilityDueTime = 5000;
        public const int WalletRefreshPeriod = 10000;
        public const int WalletSaveWalletPeriod = 120000;
    }
}
