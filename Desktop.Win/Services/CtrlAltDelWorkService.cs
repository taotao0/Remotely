using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using URemote.Shared.Utilities;
using URemote.Shared.Win32;

namespace URemote.Desktop.Win.Services
{
    public class CtrlAltDelWorkService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 파일이 있으면 돌아감.
            while (true)
            {
                if (CtrlAltDelConfigService.CheckFileExists())
                {
                    bool isSoftwareSASGeneration = false;
                    try
                    {
                        // Set Secure Attention Sequence policy to allow app to simulate Ctrl + Alt + Del.
                        var subkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true);
                        if (subkey.GetValue("SoftwareSASGeneration") == null)
                        {
                            subkey.SetValue("SoftwareSASGeneration", "3", Microsoft.Win32.RegistryValueKind.DWord);
                            isSoftwareSASGeneration = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }

                    User32.SendSAS(false);

                    try
                    {
                        if (isSoftwareSASGeneration)
                        {
                            // Remove Secure Attention Sequence policy to allow app to simulate Ctrl + Alt + Del.
                            var subkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true);
                            if (subkey.GetValue("SoftwareSASGeneration") != null)
                            {
                                subkey.DeleteValue("SoftwareSASGeneration");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex);
                    }

                    CtrlAltDelConfigService.DeleteFile();
                }
                await Task.Delay(1000);
            }
        }
    }
}
