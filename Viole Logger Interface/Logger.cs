using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Viole_Pipe;

namespace Viole_Logger_Interface
{
    public partial class Logger
    {
        public string GetLocalTime() => DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt", CultureInfo.InvariantCulture).ToString();

        public readonly string File;
        public readonly string Name;

        private readonly Pipe Pipe;

        public Logger(string File, string Name = null)
        {
            this.File = File;
            this.Name = Name;

            Pipe = new Pipe("VIOLE LOGGER");
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

        public bool Setup(int X = 0, int Y = 0, bool Hide = false)
        {
            if (System.IO.File.Exists(File) && !Pipe.Any())
            {
                var List = new List<string>();

                if (X > 0 && Y > 0)
                {
                    List.Add($"--window {X} {Y}");
                }

                if (Hide)
                {
                    List.Add("--hide");
                }

                var Process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = File,
                        Arguments = string.Join(" ", List)
                    }
                };

                Process.Start();
                Process.Dispose();

                return true;
            }

            return false;
        }

        public void Clear()
        {
            if (Pipe.Any())
            {
                new Thread(() =>  Pipe.Set(JsonConvert.SerializeObject(new ILogger { Message = "CLEAR", Color = ConsoleColor.White }))).Start();
            }
        }

        public void Close()
        {
            if (Pipe.Any())
            {
                new Thread(() =>  Pipe.Set(JsonConvert.SerializeObject(new ILogger { Message = "CLOSE", Color = ConsoleColor.White }))).Start();
            }
        }

        private void Write(string Message, ConsoleColor Color, bool Generic = true)
        {
            if (string.IsNullOrEmpty(Message))
            {
                LogGenericError($"{nameof(Message)} is null!");

                return;
            }

            if (Pipe.Any())
            {
                string Layout = string.IsNullOrEmpty(Name) ? Message : $"{Name} | {Message}";

                if (Generic)
                {
                    Layout = $"{GetLocalTime()}|";

                    if (string.IsNullOrEmpty(Name))
                        Layout += Message;
                    else
                        Layout += $"{Name}|{Message}";
                }

                try
                {
                    var Thread = new Thread(() => Pipe.Set(JsonConvert.SerializeObject(new ILogger { Message = Layout, Color = Color })));

                    Thread.Start();
                }
                catch { }
            }
            else
            {
                LogTrace(Message);
            }
        }

        private class ILogger
        {
            [JsonProperty]
            public string Message { get; set; }

            [JsonProperty]
            public ConsoleColor Color { get; set; }
        }
    }
}
