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
    class Runner
    {
        Int64 TimeLimit = 0;
        Int64 MemoryLimit = 0;

        private Int64 _UsedTime = 0;
        public Int64 UsedTime
        {
            get
            {
                return this._UsedTime;
            }
        }

        private Int64 _UsedMemory = 0;
        public Int64 UsedMemory
        {
            get
            {
                return this._UsedMemory;
            }
        }

        private int _StatusCode = 0;
        public int StatusCode
        {
            get
            {
                return this._StatusCode;
            }
        }

        private bool _IsSuccess = false;
        public bool IsSuccess
        {
            get
            {
                return this._IsSuccess;
            }
        }

        private StringBuilder _OutputData = new StringBuilder();
        public string OutputData
        {
            get
            {
                return this._OutputData.ToString();
            }
        }

        Process Exep = null;

        public Runner(Int64 timeLimit, Int64 memoryLimit)
        {
            this.TimeLimit = timeLimit;
            this.MemoryLimit = memoryLimit;
        }

        public bool RunEXE(string dirPath, string exeFileName, string inputData)
        {
            this.Exep = new Process();
            this.Exep.StartInfo.FileName = dirPath + exeFileName;
            this.Exep.StartInfo.CreateNoWindow = true;
            this.Exep.StartInfo.UseShellExecute = false;
            this.Exep.StartInfo.RedirectStandardInput = true;
            this.Exep.StartInfo.RedirectStandardOutput = true;

            try
            {
                this._StatusCode = 0;
                this._IsSuccess = true;

                //启动进程
                this.Exep.Start();

                //启动计时线程
                Thread programRunner_TimeChecker = new Thread(Thread_ProgramRunner_TimeChecker);
                programRunner_TimeChecker.Start();

                //设置最大使用内存
                
                if (this.Exep.MaxWorkingSet.ToInt64() > this.MemoryLimit)
                {
                    this._UsedMemory = this.MemoryLimit;
                    if (!this.Exep.HasExited) this.Exep.Kill();
                    throw new OutOfMemoryException();
                }
                else
                {
                    this.Exep.MaxWorkingSet = new IntPtr(this.MemoryLimit);
                    this._UsedMemory = this.Exep.PeakWorkingSet64;
                }

                //输入数据
                this.Exep.StandardInput.Write(inputData);
                this.Exep.StandardInput.Close();

                //等待进程结束
                this.Exep.WaitForExit();

                //输出数据
                if (!this.Exep.StandardOutput.EndOfStream) this._OutputData.Append(this.Exep.StandardOutput.ReadToEnd());
            }
            catch (OutOfMemoryException)
            {
                if (!this.Exep.HasExited) this.Exep.Kill();
                if (this._IsSuccess)
                    this._StatusCode = Const._StatusCode_MemoryLimitExceeded;
                this._IsSuccess = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (!this.Exep.HasExited) this.Exep.Kill();
                if (this._IsSuccess)
                    this._StatusCode = Const._StatusCode_UnknownStatus;
                this._IsSuccess = false;
            }
            finally
            {
                if (!Exep.HasExited) Exep.Kill();

                if (this._IsSuccess)
                {
                    this._UsedTime = Convert.ToInt64(this.Exep.TotalProcessorTime.TotalMilliseconds);
                    this._UsedMemory = Convert.ToInt64(this.Exep.PeakWorkingSet64);
                }
            }
            return this._IsSuccess;
        }


        public bool RunClass(string dirPath, string classFileName, string inputData)
        {
            this.Exep = new Process();
            this.Exep.StartInfo.FileName = "java";
            this.Exep.StartInfo.Arguments = "-cp " + dirPath + " " + classFileName.Split('.')[0];
            this.Exep.StartInfo.CreateNoWindow = true;
            this.Exep.StartInfo.UseShellExecute = false;
            this.Exep.StartInfo.RedirectStandardInput = true;
            this.Exep.StartInfo.RedirectStandardOutput = true;

            try
            {
                this._StatusCode = 0;
                this._IsSuccess = true;

                //启动进程
                this.Exep.Start();

                //启动计时线程
                Thread programRunner_TimeChecker = new Thread(Thread_ProgramRunner_TimeChecker);
                programRunner_TimeChecker.Start();

                //设置最大使用内存

                if (this.Exep.MaxWorkingSet.ToInt64() > this.MemoryLimit)
                {
                    this._UsedMemory = this.MemoryLimit;
                    if (!this.Exep.HasExited) this.Exep.Kill();
                    throw new OutOfMemoryException();
                }
                else
                {
                    this.Exep.MaxWorkingSet = new IntPtr(this.MemoryLimit);
                    this._UsedMemory = this.Exep.PeakWorkingSet64;
                }

                //输入数据
                this.Exep.StandardInput.Write(inputData);
                this.Exep.StandardInput.Close();

                //等待进程结束
                this.Exep.WaitForExit();

                //输出数据
                if (!this.Exep.StandardOutput.EndOfStream) this._OutputData.Append(this.Exep.StandardOutput.ReadToEnd());
            }
            catch (OutOfMemoryException)
            {
                if (!this.Exep.HasExited) this.Exep.Kill();
                if (this._IsSuccess)
                    this._StatusCode = Const._StatusCode_MemoryLimitExceeded;
                this._IsSuccess = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (!this.Exep.HasExited) this.Exep.Kill();
                if (this._IsSuccess)
                    this._StatusCode = Const._StatusCode_UnknownStatus;
                this._IsSuccess = false;
            }
            finally
            {
                if (!Exep.HasExited) Exep.Kill();

                if (this._IsSuccess)
                {
                    this._UsedTime = Convert.ToInt64(this.Exep.TotalProcessorTime.TotalMilliseconds);
                    this._UsedMemory = Convert.ToInt64(this.Exep.PeakWorkingSet64);
                }
            }
            return this._IsSuccess;
        }

        private void Thread_ProgramRunner_TimeChecker()
        {
            while (!this.Exep.HasExited)
            {
                if (this.Exep.TotalProcessorTime.TotalMilliseconds > this.TimeLimit)
                {
                    if (!this.Exep.HasExited) this.Exep.Kill();
                    this._IsSuccess = false;
                    this._StatusCode = Const._StatusCode_TimeLimitExceeded;
                    this._UsedTime = this.TimeLimit;
                    return;
                }
            }
        }
    }
}
