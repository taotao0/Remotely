using Microsoft.Extensions.DependencyInjection;
using URemote.Desktop.Core;
using URemote.Desktop.Core.Interfaces;
using URemote.Desktop.Core.Services;
using URemote.Shared.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace URemote.Desktop.Win.Services
{
    public class ShutdownServiceWin : IShutdownService
    {
        public async Task Shutdown()
        {
            try
            {
                Logger.Write($"Exiting process ID {Environment.ProcessId}.");
                var casterSocket = ServiceContainer.Instance.GetRequiredService<ICasterSocket>();
                await casterSocket.DisconnectAllViewers();
                await casterSocket.Disconnect();
                System.Windows.Forms.Application.Exit();
                App.Current.Dispatcher.Invoke(() =>
                {
                    App.Current.Shutdown();
                });
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }
    }
}
