using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACMAnswerChecker
{
    static class Compiler
    {
        public static Process Exep { get; private set; }
        public static StringBuilder Error { get; private set; }
        public static string ErrorInfo { get; private set; }

        public static int Compile(string workingDirectory, Answer thisAnswer)
        {
            string srcFilePath;

            Exep = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            switch (thisAnswer.LanguageCode)
            {
                case Const._LanguageCode_C:
                    srcFilePath = workingDirectory + "Main.c";
                    Exep.StartInfo.FileName = "gcc";
                    Exep.StartInfo.Arguments = srcFilePath + " -o " + workingDirectory + "Main.exe" + " -O2 -Wall -lm --static -std=c99 -DONLINE_JUDGE";
                    break;
                case Const._LanguageCode_CPP:
                    srcFilePath = workingDirectory + "Main.cpp";
                    Exep.StartInfo.FileName = "g++";
                    Exep.StartInfo.Arguments = srcFilePath + " -o " + workingDirectory + "Main.exe" + " -O2 -Wall -lm --static -DONLINE_JUDGE";
                    break;
                case Const._LanguageCode_Java:
                    srcFilePath = workingDirectory + "Main.java";
                    Exep.StartInfo.FileName = "javac";
                    Exep.StartInfo.Arguments = "-J-Xms32m -J-Xmx256m " + srcFilePath;
                    break;
                default:
                    throw new Exception("不支持的语言类型");
            }

            var streamWriter = new StreamWriter(File.Create(srcFilePath));
            streamWriter.Write(thisAnswer.SourceCode);
            streamWriter.Close();

            Exep.Start();
            
            //启动错误流监控线程
            var threadWatchErrorStream = new Thread(WatchErrorStream);
            threadWatchErrorStream.Start();

            Exep.WaitForExit();

            var exitCode = Exep.ExitCode;
            if (exitCode != 0)
            {
                ErrorInfo = Error.ToString();
            }

            return exitCode;
        }

        private static void WatchErrorStream()
        {
            while (!Exep.StandardError.EndOfStream)
            {
                Error.AppendLine(Exep.StandardError.ReadLine());
            }
        }

    }
}
