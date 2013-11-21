using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ACMAnswerChecker
{
    class Program
    {
        private static int serverPort = 8384;   //端口  
        static Socket serverSocket;

        static void Main(string[] args)
        {
            //服务器IP地址
            IPAddress ip = IPAddress.Parse("127.0.0.1");

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //绑定IP地址：端口
            serverSocket.Bind(new IPEndPoint(ip, serverPort));

            //设定最多10个排队连接请求
            serverSocket.Listen(10);

            Console.WriteLine("[{0}] Server Running At {1}", DateTime.Now.ToString(), serverSocket.LocalEndPoint.ToString());

            //通过Clientsoket发送数据  
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();
        }

        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();
                Console.WriteLine("[{0}] Client Connected.", DateTime.Now.ToString());
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }
        }

        private static void ReceiveMessage(object _clientSocket)
        {
            Socket clientSocket = (Socket)_clientSocket;
            byte[] buff = new byte[1024];

            StringBuilder receivedString = new StringBuilder();
            while (true)
            {
                try
                {
                    int receiveNumber = clientSocket.Receive(buff);
                    foreach (char thisChar in Encoding.ASCII.GetString(buff, 0, receiveNumber))
                    {
                        if (thisChar != '\n')
                        {
                            receivedString.Append(thisChar);
                        }
                        else
                        {
                            string receivedJSON = Encoding.ASCII.GetString(Convert.FromBase64String(receivedString.ToString()));

                            JObject jObject = JObject.Parse(receivedJSON);

                            Int64 answerID = 0;
                            Int64 problemID = 0;
                            int languageCode = 0;
                            string dirPath = "";
                            string srcFileName = "";
                            string inputData = "";
                            string stdOutputData = "";
                            Int64 timeLimit = 0;
                            Int64 memoryLimit = 0;

                            string binFileName = "";
                            string outputData = "";

                            foreach (var thisKV in jObject)
                            {
                                switch (thisKV.Key)
                                {
                                    case "answerID":
                                        answerID = Convert.ToInt64(Convert.ToString(thisKV.Value));
                                        break;
                                    case "problemID":
                                        problemID = Convert.ToInt64(Convert.ToString(thisKV.Value));
                                        break;
                                    case "languageCode":
                                        languageCode = Convert.ToInt32(Convert.ToString(thisKV.Value));
                                        break;
                                    case "dirPath":
                                        dirPath = Convert.ToString(thisKV.Value);
                                        break;
                                    case "srcFileName":
                                        srcFileName = Convert.ToString(thisKV.Value);
                                        break;
                                    case "inputData":
                                        inputData = Convert.ToString(thisKV.Value);
                                        break;
                                    case "stdOutputData":
                                        stdOutputData = Convert.ToString(thisKV.Value);
                                        break;
                                    case "timeLimit":
                                        timeLimit = Convert.ToInt64(Convert.ToString(thisKV.Value)) * 1000;
                                        break;
                                    case "memoryLimit":
                                        memoryLimit = Convert.ToInt64(Convert.ToString(thisKV.Value)) * 1000;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            int StatusCode = 0;
                            string Info = "";
                            Int64 UsedTime = 0;
                            Int64 UsedMemory = 0;

                            dirPath = @"D:/wamp/www/siseoj/" + dirPath;

                            Compiler compiler = new Compiler();
                            Runner runner = new Runner(timeLimit, memoryLimit);
                            Checker checker = new Checker();
                            DBUpdater dbUpdater = new DBUpdater();

                            switch (languageCode)
                            {
                                case Const._LanguageCode_C:
                                    compiler.GCC(dirPath, srcFileName);
                                    binFileName = "Main.exe";
                                    break;
                                case Const._LanguageCode_CPP:
                                    compiler.GPP(dirPath, srcFileName);
                                    binFileName = "Main.exe";
                                    break;
                                case Const._LanguageCode_Java:
                                    compiler.JDK(dirPath, srcFileName);
                                    binFileName = "Main.class";
                                    break;
                                default:
                                    StatusCode = Const._StatusCode_SystemError;
                                    dbUpdater.Update(answerID, problemID, StatusCode, Info, UsedTime, UsedMemory, inputData, outputData);
                                    throw new Exception("不支持的语言类型");
                            }

                            if (compiler.ExitCode != 0)
                            {
                                StatusCode = Const._StatusCode_CompileError;
                                Info = compiler.StdErr;
                            }
                            else
                            {
                                if (languageCode == Const._LanguageCode_C || languageCode == Const._LanguageCode_CPP)
                                {
                                    runner.RunEXE(dirPath, binFileName, inputData);
                                }
                                else if (languageCode == Const._LanguageCode_Java)
                                {
                                    runner.RunClass(dirPath, binFileName, inputData);
                                }
                                else
                                {
                                    throw new Exception("不支持的语言类型");
                                }

                                UsedTime = runner.UsedTime;
                                UsedMemory = runner.UsedMemory;
                                outputData = runner.OutputData;

                                if (runner.StatusCode != 0)
                                {
                                    StatusCode = runner.StatusCode;
                                }
                                else
                                {
                                    checker.Check(stdOutputData, outputData);
                                    StatusCode = checker.StatusCode;
                                }
                            }
                            
                            dbUpdater.Update(answerID, problemID, StatusCode, Info, UsedTime, UsedMemory, inputData, outputData);

                            JObject Result = new JObject();
                            Result.Add("AnswerID", answerID);
                            Result.Add("StatusCode", StatusCode);
                            Result.Add("Info", Info);
                            Result.Add("UsedTime", UsedTime);
                            Result.Add("UsedMemory", UsedMemory);

                            Console.WriteLine("[{0}] {1}", DateTime.Now.ToString(), JsonConvert.SerializeObject(Result));

                            clientSocket.Shutdown(SocketShutdown.Both);
                            clientSocket.Close();

                            Console.WriteLine("[{0}] Client Disconnected.", DateTime.Now.ToString());

                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    break;
                }
            }
        }

    }
}
