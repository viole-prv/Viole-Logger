using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Viole_Logger_Interactive
{
    public partial class Logger
    {
        public string GetLocalTime() => DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt", CultureInfo.InvariantCulture).ToString();

        public readonly string Name;

        public Logger(string Name = null)
        {
            this.Name = Name;
        }

        public void LogGenericInfo(object Message, [CallerMemberName] string MethodName = null)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!", MethodName);

                return;
            }

            Write($"{MethodName}() {Message}", ConsoleColor.White);
        }

        public void LogInfo(object Message)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!");

                return;
            }

            Write(Message.ToString(), ConsoleColor.White, false);
        }

        public void LogGenericDebug(object Message, [CallerMemberName] string MethodName = null)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!", MethodName);

                return;
            }

            Write($"{MethodName}() {Message}", ConsoleColor.Gray);
        }

        public void LogDebug(object Message)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!");

                return;
            }

            Write(Message.ToString(), ConsoleColor.Gray, false);
        }

        public static void LogTrace(object Message)
        {
            if (Message == null)
            {
                Trace.WriteLine($"{nameof(Message)} is null!");

                return;
            }

            var T = new Type[] {
                typeof(bool),
                typeof(byte),
                typeof(sbyte),
                typeof(char),
                typeof(decimal),
                typeof(double),
                typeof(float),
                typeof(int),
                typeof(uint),
                // typeof(nint),
                // typeof(nuint),
                typeof(long),
                typeof(ulong),
                typeof(short),
                typeof(ushort),

                typeof(string)
            };

            if (!T.Contains(Message.GetType()))
            {
                Message = $"\n{JsonConvert.SerializeObject(Message, Formatting.Indented)}";
            }

            Trace.WriteLine(Message);
        }

        public void LogGenericError(object Message, [CallerMemberName] string MethodName = null)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!", MethodName);

                return;
            }

            Write($"{MethodName}() {Message}", ConsoleColor.Yellow);
        }

        public void LogError(object Message)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!");

                return;
            }

            Write(Message.ToString(), ConsoleColor.Yellow, false);
        }

        public void LogGenericWarning(object Message, [CallerMemberName] string MethodName = null)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!", MethodName);

                return;
            }

            Write($"{MethodName}() {Message}", ConsoleColor.Magenta);
        }

        public void LogWarning(object Message)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!");

                return;
            }

            Write(Message.ToString(), ConsoleColor.Magenta, false);
        }

        public void LogGenericObject(object Message, [CallerMemberName] string MethodName = null)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!", MethodName);

                return;
            }

            Write($"{MethodName}()\n{JsonConvert.SerializeObject(Message, Formatting.Indented)}", ConsoleColor.Gray);
        }

        public void LogObject(object Message)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!");

                return;
            }

            Write(JsonConvert.SerializeObject(Message, Formatting.Indented), ConsoleColor.Gray, false);
        }

        public void LogGenericObject<T>(object Message, [CallerMemberName] string MethodName = null)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!", MethodName);

                return;
            }

            Write($"{MethodName}()\n{JsonConvert.SerializeObject(Message, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new JsonPropertyResolver<T>() })}", ConsoleColor.Gray);
        }

        public void LogObject<T>(object Message)
        {
            if (Message == null)
            {
                LogGenericError($"{nameof(Message)} is null!");

                return;
            }

            Write(JsonConvert.SerializeObject(Message, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new JsonPropertyResolver<T>() }), ConsoleColor.Gray, false);
        }

        public void LogGenericException(Exception Exception, [CallerMemberName] string MethodName = null)
        {
            if (Exception == null)
            {
                LogGenericError($"{nameof(Exception)} is null!", MethodName);

                return;
            }

            Write($"{MethodName}() {Exception.Message} {Helper.GetStackFrame(Exception)}", ConsoleColor.Yellow);
        }
             
        public void LogException(Exception Exception)
        {
            if (Exception == null)
            {
                LogGenericError($"{nameof(Exception)} is null!");

                return;
            }

            Write($"{Exception.Message} {Helper.GetStackFrame(Exception)}", ConsoleColor.Yellow, false);
        }

        public void LogGenericException([CallerMemberName] string MethodName = null)
        {
            int Error = Marshal.GetLastWin32Error();

            if (Error > 0)
            {
                LogGenericException(new Win32Exception(Error), MethodName);
            }
        }

        public void LogException()
        {
            int Error = Marshal.GetLastWin32Error();

            if (Error > 0)
            {
                LogException(new Win32Exception(Error));
            }
        }

        private void Write(string Message, ConsoleColor Color, bool Generic = true)
        {
            if (string.IsNullOrEmpty(Message))
            {
                LogGenericError(nameof(Message));

                return;
            }

            string Layout = string.IsNullOrEmpty(Name) ? Message : $"{Name} | {Message}";

            if (Generic)
            {
                Layout = $"{GetLocalTime()}|";

                if (string.IsNullOrEmpty(Name))
                    Layout += Message;
                else
                    Layout += $"{Name}|{Message}";
            }

            Console.ForegroundColor = Color;
            Console.WriteLine(Layout);
            Console.ResetColor();
        }
    }
}
