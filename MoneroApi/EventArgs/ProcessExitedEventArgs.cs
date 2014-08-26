using System;

namespace Jojatekok.MoneroAPI
{
    public class ProcessExitedEventArgs : EventArgs
    {
        public int ExitCode { get; private set; }

        internal ProcessExitedEventArgs(int exitCode)
        {
            ExitCode = exitCode;
        }
    }
}
