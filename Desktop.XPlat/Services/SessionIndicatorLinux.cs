using Avalonia.Controls;
using Avalonia.Threading;
using URemote.Desktop.Core.Interfaces;
using URemote.Desktop.XPlat.Views;

namespace URemote.Desktop.XPlat.Services
{
    public class SessionIndicatorLinux : ISessionIndicator
    {
        public void Show()
        {
            Dispatcher.UIThread.Post(() =>
            {
                var indicatorWindow = new SessionIndicatorWindow();
                indicatorWindow.Show();
            });
        }
    }
}
