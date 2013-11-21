using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACMAnswerChecker
{
    class Compiler
    {
        private int _ExitCode;
        public int ExitCode
        {
            get
            {
                return this._ExitCode;
            }
        }

        private string _StdErr;
        public string StdErr
        {
            get
            {
                return this._StdErr;
            }
        }


        public bool GCC(string dirPath, string SrcFileName)
        {
            Process exep = new Process();
            exep.StartInfo.FileName = "gcc";
            exep.StartInfo.Arguments = dirPath + SrcFileName + " -o " + dirPath + "Main.exe -O2 -Wall -lm --static -std=c99 -DONLINE_JUDGE";
            exep.StartInfo.CreateNoWindow = true;
            exep.StartInfo.UseShellExecute = false;
            exep.StartInfo.RedirectStandardOutput = true;
            exep.StartInfo.RedirectStandardError = true;
            exep.Start();
            exep.WaitForExit();

            this._ExitCode = exep.ExitCode;

            if (this._ExitCode == 0)
            {
                return true;
            }
            else
            {
                this._StdErr = exep.StandardError.ReadToEnd();
                return false;
            }
        }

        public bool GPP(string dirPath, string SrcFileName)
        {
            Process exep = new Process();
            exep.StartInfo.FileName = "g++";
            exep.StartInfo.Arguments = dirPath + SrcFileName + " -o " + dirPath + "Main.exe -O2 -Wall -lm --static -DONLINE_JUDGE";
            exep.StartInfo.CreateNoWindow = true;
            exep.StartInfo.UseShellExecute = false;
            exep.StartInfo.RedirectStandardOutput = true;
            exep.StartInfo.RedirectStandardError = true;
            exep.Start();
            exep.WaitForExit();

            this._ExitCode = exep.ExitCode;

            if (this._ExitCode == 0)
            {
                return true;
            }
            else
            {
                this._StdErr = exep.StandardError.ReadToEnd();
                return false;
            }
        }

        public bool JDK(string dirPath, string SrcFileName)
        {
            Process exep = new Process();
            exep.StartInfo.FileName = "javac";
            exep.StartInfo.Arguments = "-J-Xms32m -J-Xmx256m " + dirPath + SrcFileName;
            exep.StartInfo.CreateNoWindow = true;
            exep.StartInfo.UseShellExecute = false;
            exep.StartInfo.RedirectStandardOutput = true;
            exep.StartInfo.RedirectStandardError = true;
            exep.Start();
            exep.WaitForExit();

            this._ExitCode = exep.ExitCode;

            if (this._ExitCode == 0)
            {
                return true;
            }
            else
            {
                this._StdErr = exep.StandardError.ReadToEnd();
                return false;
            }
        }
    }
}
