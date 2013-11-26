using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACMAnswerChecker
{
    static class Compiler
    {

        public static string ErrorInfo { get; private set; }

        public static int Compile(string workingDirectory, Answer thisAnswer)
        {
            string srcFilePath;

            var exep = new Process
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
                    exep.StartInfo.FileName = "gcc";
                    exep.StartInfo.Arguments = srcFilePath + " -o " + workingDirectory + "Main.exe" + " -O2 -Wall -lm --static -std=c99 -DONLINE_JUDGE";
                    break;
                case Const._LanguageCode_CPP:
                    srcFilePath = workingDirectory + "Main.cpp";
                    exep.StartInfo.FileName = "g++";
                    exep.StartInfo.Arguments = srcFilePath + " -o " + workingDirectory + "Main.exe" + " -O2 -Wall -lm --static -DONLINE_JUDGE";
                    break;
                case Const._LanguageCode_Java:
                    srcFilePath = workingDirectory + "Main.java";
                    exep.StartInfo.FileName = "javac";
                    exep.StartInfo.Arguments = "-J-Xms32m -J-Xmx256m " + srcFilePath;
                    break;
                default:
                    throw new Exception("不支持的语言类型");
            }

            var streamWriter = new StreamWriter(File.Create(srcFilePath));
            streamWriter.Write(thisAnswer.SourceCode);
            streamWriter.Close();

            exep.Start();
            exep.WaitForExit();

            var exitCode = exep.ExitCode;
            if (exitCode != 0)
            {
                ErrorInfo = exep.StandardError.ReadToEnd();
            }

            return exitCode;
        }

    }
}
