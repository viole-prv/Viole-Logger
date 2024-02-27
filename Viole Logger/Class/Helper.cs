using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.InteropServices;

namespace Viole_Logger
{
    public partial class Helper
    {
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMAXIMIZED = 3;

        public const int SWP_NOSIZE = 0x0001;

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        
        public static bool IsValidJson(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input)) { return false; }

            Input = Input.Trim();

            if ((Input.StartsWith("{") && Input.EndsWith("}")) ||
                (Input.StartsWith("[") && Input.EndsWith("]")))
            {
                try
                {
                    var Token = JToken.Parse(Input);

                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
