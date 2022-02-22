using System;
using System.Threading.Tasks;

namespace URemote.Agent.Interfaces
{
    public interface IUpdater : IDisposable
    {
        Task BeginChecking();
        Task CheckForUpdates();
        Task InstallLatestVersion();
    }
}