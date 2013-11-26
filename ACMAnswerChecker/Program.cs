using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACMAnswerChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Runner.InitExitCodeDictionary();
            DatabaseConnector.Init("localhost", "sise_acm_oj", "sise_acm_oj", "sise_acm_oj_sql");
            SocketConnector.Init("127.0.0.1", 8384);
            while (SocketConnector.ServerSocket.Connected)
            {
                Console.ReadLine();
            }
        }

    }
}
