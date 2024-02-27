using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;

namespace Viole_Logger_Interactive
{
    public partial class Logger
    {
        public class Helper
        {
            public static bool IsValidJson(string _)
            {
                if (string.IsNullOrWhiteSpace(_)) { return false; }

                _ = _.Trim();

                if ((_.StartsWith("{") && _.EndsWith("}")) ||
                    (_.StartsWith("[") && _.EndsWith("]")))
                {
                    try
                    {
                        var Token = JToken.Parse(_);

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

            public static string GetStackFrame(Exception _)
            {
                string X = string.Empty;

                if (!string.IsNullOrEmpty(_.StackTrace))
                {
                    X = _.StackTrace.Trim();
                }

                try
                {
                    var StackTrace = new StackTrace(_, true);

                    if (StackTrace.FrameCount > 0)
                    {
                        var GetFrame = StackTrace.GetFrame(StackTrace.FrameCount - 1);

                        string FileName = GetFrame.GetFileName();

                        X = $"{GetFrame.GetMethod().ReflectedType.FullName}.{GetFrame.GetMethod().Name}({string.Join(", ", GetFrame.GetMethod().GetParameters().Select(p => $"{p.ParameterType.FullName} {p.Name}").ToArray())}) - {FileName ?? string.Empty}{(GetFrame.GetFileLineNumber() == 0 ? string.Empty : $":line {GetFrame.GetFileLineNumber()}")}";
                    }
                }
                catch { }

                return X;
            }
        }
    }
}
