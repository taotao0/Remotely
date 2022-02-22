using System;
using System.Diagnostics;
using URemote.Shared.Utilities;

namespace URemote.Desktop.Win.Services
{
    public class CtrlAltDelConfigService
    {
        private static string Path => System.IO.Path.Combine("C:\\Windows\\Temp", "URemote_CtrlAltDel.cfg"); // [KJB TEST] Desktop CtrlAltDel - 리눅스는 경로 우짜지..
        //private static string Path => System.IO.Path.Combine(System.IO.Path.GetTempPath(), "URemote_CtrlAltDel.cfg"); // exe 랑 service랑 temp 위치가 다름.

        /// <summary>
        /// 파일 있으면 true
        /// </summary>
        /// <returns></returns>
        public static bool CheckFileExists()
        {
            if (!System.IO.File.Exists(Path))
            {
                // 없음 
                return false;
            }
            else
            {
                // 존재
                return true;
            }
        }
        public static void CreateFile()
        {
            try
            {
                if (!System.IO.File.Exists(Path))
                {
                    System.IO.File.Create(Path).Close();

                    if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                    {
                        Process.Start("sudo", $"chmod 777 {Path}").WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write("CtrlAltDelService CreateFile Exception! \n\n" + ex.ToString());
            }
            finally
            {
            }
        }

        public static void DeleteFile()
        {
            try
            {
                if (System.IO.File.Exists(Path))
                {
                    System.IO.File.Delete(Path);
                }
            }
            catch { }
            finally
            {
            }
        }
    }
}
