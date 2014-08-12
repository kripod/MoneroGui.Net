using Jojatekok.MoneroAPI.RpcManagers.AccountManager.Json.Responses;

namespace Jojatekok.MoneroAPI
{
    public class BalanceChangingEventArgs : ValueChangingEventArgs<Balance>
    {
        internal BalanceChangingEventArgs(Balance newValue) : base(newValue)
        {

        }
    }
}
