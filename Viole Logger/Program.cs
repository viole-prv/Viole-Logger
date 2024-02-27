using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipes;
using System.Windows.Media.Media3D;

namespace Viole_Logger
{
    public partial class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
			Console.Title = "VIOLE LOGGER";

            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--window")
                    {
                        if (int.TryParse(args[i + 1], out int X) && int.TryParse(args[i + 2], out int Y))
                        {
                            Helper.SetWindowPos(Helper.GetConsoleWindow(), IntPtr.Zero, X, Y, 0, 0, Helper.SWP_NOSIZE);
                        }
                    }
                    else if(args[i] == "--hide")
                    {
                        Helper.ShowWindowAsync(Helper.GetConsoleWindow(), Helper.SW_SHOWMINIMIZED);
                    }
                }

                using (var NamedPipeServerStream = new NamedPipeServerStream("VIOLE LOGGER", PipeDirection.InOut))
				{
					using (var StreamReader = new StreamReader(NamedPipeServerStream))
					{
                        while (true)
                        {
                            try
                            {
                                NamedPipeServerStream.WaitForConnection();

                                string Line = StreamReader.ReadLine();

                                if (string.IsNullOrEmpty(Line)) continue;

                                if (Helper.IsValidJson(Line))
                                {
                                    var _ = JsonConvert.DeserializeObject<ILogger>(Line);

                                    if (_ == null || string.IsNullOrEmpty(_.Message)) return;

                                    switch (_.Message)
                                    {
                                        case "CLEAR":
                                            Console.Clear();

                                            break;
                                        case "CLOSE":
                                            Environment.Exit(0);

                                            break;
                                        default:
                                            Console.ForegroundColor = _.Color;
                                            Console.WriteLine(_.Message);

                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine(Line);
                                }
                            }
                            catch (Exception e)
                            {
                                Console.BackgroundColor = ConsoleColor.Red;
                                Console.Clear();
                                Console.WriteLine($"[SYSTEM] ERROR: {e.Message}");
                                Console.ReadKey();
                            }
                            finally
                            {
                                NamedPipeServerStream.Disconnect();
                            }
                        }
                    }
				}
			}
			catch (Exception e)
			{
				Console.BackgroundColor = ConsoleColor.Red;
				Console.Clear();
				Console.WriteLine($"[SYSTEM] ERROR: {e.Message}");
				Console.ReadKey();
			}
		}

        public class ILogger
        {
            [JsonProperty]
            public string Message { get; set; }

            [JsonProperty]
            public ConsoleColor Color { get; set; }
        }
    }
}
