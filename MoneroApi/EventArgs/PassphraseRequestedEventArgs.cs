using System;

namespace Jojatekok.MoneroAPI
{
    public class PassphraseRequestedEventArgs : EventArgs
    {
        public bool IsFirstTime { get; private set; }

        internal PassphraseRequestedEventArgs(bool isFirstTime)
        {
            IsFirstTime = isFirstTime;
        }
    }
}
