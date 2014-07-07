namespace Jojatekok.MoneroAPI.ProcessManagers
{
    public interface IBaseProcessManager
    {
        void Start();
        void Stop();
        void Restart();
    }
}
