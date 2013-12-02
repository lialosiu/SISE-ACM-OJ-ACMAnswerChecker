using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ACMAnswerChecker
{
    class Program
    {
        public static string OJWebServerURL { get; private set; }
        public static string Key { get; private set; }

        static void Main(string[] args)
        {
            var server = "";
            var database = "";
            var uid = "";
            var pwd = "";
            var port = 0;

            var configStreamReader = File.OpenText(@"config.txt");
            while (!configStreamReader.EndOfStream)
            {
                var thisLine = configStreamReader.ReadLine();
                var regex = new Regex("^(.*)=(.*)$");
                if (thisLine != null)
                {
                    if (regex.IsMatch(thisLine))
                    {
                        var thisMatch = regex.Match(thisLine);
                        var k = thisMatch.Groups[1].Value;
                        var v = thisMatch.Groups[2].Value;
                        switch (k)
                        {
                            case "Server":
                                server = v;
                                break;
                            case "Database":
                                database = v;
                                break;
                            case "UID":
                                uid = v;
                                break;
                            case "PWD":
                                pwd = v;
                                break;
                            case "Port":
                                port = Convert.ToInt32(v);
                                break;
                            case "OJWebServerURL":
                                OJWebServerURL = v;
                                break;
                            case "Key":
                                Key = v;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            configStreamReader.Close();

            Runner.InitExitCodeDictionary();
            DatabaseConnector.Init(server, database, uid, pwd);
            SocketConnector.Init("0.0.0.0", port);
            while (SocketConnector.ServerSocket.Connected)
            {
                Console.ReadLine();
            }
        }

    }
}
