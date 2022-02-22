using Microsoft.Extensions.DependencyInjection;
using URemote.Desktop.Core;
using URemote.Desktop.Core.Interfaces;
using URemote.Desktop.Core.Services;
using URemote.Shared.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace URemote.Desktop.XPlat.Services
{
    public class ShutdownServiceLinux : IShutdownService
    {
        public async Task Shutdown()
        {
            Logger.Debug($"Exiting process ID {Environment.ProcessId}.");
            var casterSocket = ServiceContainer.Instance.GetRequiredService<ICasterSocket>();
            await casterSocket.DisconnectAllViewers();
            Environment.Exit(0);
        }
    }
}
