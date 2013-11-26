﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACMAnswerChecker
{
    static class Runner
    {
        private static readonly Dictionary<long, string> ExitCodeDictionary = new Dictionary<long, string>();
        
        public static Process Exep { get; private set; }
        public static Int64 TimeLimit { get; private set; }
        public static Int64 MemoryLimit { get; private set; }
        public static Int64 UsedTime { get; private set; }
        public static Int64 UsedMemory { get; private set; }
        public static Int16 StatusCode { get; private set; }

        public static void InitExitCodeDictionary()
        {
            ExitCodeDictionary.Add(-1073741510, "CONTROL_C_EXIT");
            ExitCodeDictionary.Add(1073807369, "DBG_COMMAND_EXCEPTION");
            ExitCodeDictionary.Add(65538, "DBG_CONTINUE");
            ExitCodeDictionary.Add(1073807368, "DBG_CONTROL_BREAK");
            ExitCodeDictionary.Add(1073807365, "DBG_CONTROL_C");
            ExitCodeDictionary.Add(65537, "DBG_EXCEPTION_HANDLED");
            ExitCodeDictionary.Add(-2147418111, "DBG_EXCEPTION_NOT_HANDLED");
            ExitCodeDictionary.Add(1073807364, "DBG_TERMINATE_PROCESS");
            ExitCodeDictionary.Add(1073807363, "DBG_TERMINATE_THREAD");
            ExitCodeDictionary.Add(-1073741819, "EXCEPTION_ACCESS_VIOLATION");
            ExitCodeDictionary.Add(-1073741684, "EXCEPTION_ARRAY_BOUNDS_EXCEEDED");
            ExitCodeDictionary.Add(-2147483645, "EXCEPTION_BREAKPOINT");
            ExitCodeDictionary.Add(-2147483646, "EXCEPTION_DATATYPE_MISALIGNMENT");
            ExitCodeDictionary.Add(-1073741683, "EXCEPTION_FLT_DENORMAL_OPERAND");
            ExitCodeDictionary.Add(-1073741682, "EXCEPTION_FLT_DIVIDE_BY_ZERO");
            ExitCodeDictionary.Add(-1073741681, "EXCEPTION_FLT_INEXACT_RESULT");
            ExitCodeDictionary.Add(-1073741680, "EXCEPTION_FLT_INVALID_OPERATION");
            ExitCodeDictionary.Add(-1073741679, "EXCEPTION_FLT_OVERFLOW");
            ExitCodeDictionary.Add(-1073741678, "EXCEPTION_FLT_STACK_CHECK");
            ExitCodeDictionary.Add(-1073741677, "EXCEPTION_FLT_UNDERFLOW");
            ExitCodeDictionary.Add(-2147483647, "EXCEPTION_GUARD_PAGE");
            ExitCodeDictionary.Add(-1073741795, "EXCEPTION_ILLEGAL_INSTRUCTION");
            ExitCodeDictionary.Add(-1073741676, "EXCEPTION_INT_DIVIDE_BY_ZERO");
            ExitCodeDictionary.Add(-1073741675, "EXCEPTION_INT_OVERFLOW");
            ExitCodeDictionary.Add(-1073741786, "EXCEPTION_INVALID_DISPOSITION");
            ExitCodeDictionary.Add(-1073741816, "EXCEPTION_INVALID_HANDLE");
            ExitCodeDictionary.Add(-1073741818, "EXCEPTION_IN_PAGE_ERROR");
            ExitCodeDictionary.Add(-1073741787, "EXCEPTION_NONCONTINUABLE_EXCEPTION");
            ExitCodeDictionary.Add(-1073741674, "EXCEPTION_PRIV_INSTRUCTION");
            ExitCodeDictionary.Add(-2147483644, "EXCEPTION_SINGLE_STEP");
            ExitCodeDictionary.Add(-1073741571, "EXCEPTION_STACK_OVERFLOW");
            ExitCodeDictionary.Add(128, "STATUS_ABANDONED_WAIT_0");
            ExitCodeDictionary.Add(-1073741801, "STATUS_NO_MEMORY");
            ExitCodeDictionary.Add(259, "STATUS_PENDING");
            ExitCodeDictionary.Add(-1073741111, "STATUS_REG_NAT_CONSUMPTION");
            ExitCodeDictionary.Add(1073741829, "STATUS_SEGMENT_NOTIFICATION");
            ExitCodeDictionary.Add(-1072365553, "STATUS_SXS_EARLY_DEACTIVATION");
            ExitCodeDictionary.Add(-1072365552, "STATUS_SXS_INVALID_DEACTIVATION");
            ExitCodeDictionary.Add(258, "STATUS_TIMEOUT");
            ExitCodeDictionary.Add(192, "STATUS_USER_APC");
        }

        public static int Run(string workingDirectory, Answer thisAnswer, Problem thisProblem)
        {
            Exep = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            switch (thisAnswer.LanguageCode)
            {
                case Const._LanguageCode_C:
                case Const._LanguageCode_CPP:
                    Exep.StartInfo.FileName = workingDirectory + "Main.exe";
                    TimeLimit = thisProblem.TimeLimitNormal * 1000;
                    MemoryLimit = thisProblem.MemoryLimitNormal * 1000;
                    break;
                case Const._LanguageCode_Java:
                    Exep.StartInfo.FileName = "java";
                    Exep.StartInfo.Arguments = "-cp " + workingDirectory + " " + "Main";
                    TimeLimit = thisProblem.TimeLimitJava * 1000;
                    MemoryLimit = thisProblem.MemoryLimitJava * 1000;
                    break;
                default:
                    throw new Exception("不支持的语言类型");
            }

            try
            {
                //启动进程
                Exep.Start();

                StatusCode = Const._StatusCode_Accepted;

                //启动计时线程
                var threadTimechecker = new Thread(Watch);
                threadTimechecker.Start();

                //设置最大使用内存

                if (Exep.MaxWorkingSet.ToInt64() < MemoryLimit)
                {
                    Exep.MaxWorkingSet = new IntPtr(MemoryLimit);
                }
                else
                {
                    if (!Exep.HasExited) Exep.Kill();
                    throw new OutOfMemoryException();
                }


                //输入数据
                Exep.StandardInput.Write(thisProblem.StandardInput);
                Exep.StandardInput.Close();

                //等待进程结束
                Exep.WaitForExit();

                //输出数据
                thisAnswer.InputData = thisProblem.StandardInput;
                thisAnswer.OutputData = Exep.StandardOutput.ReadToEnd();
            }
            catch (OutOfMemoryException)
            {
                StatusCode = Const._StatusCode_MemoryLimitExceeded;
            }
            catch (Exception)
            {
                StatusCode = Const._StatusCode_SystemError;
                throw;
            }
            finally
            {
                if (!Exep.HasExited) Exep.Kill();
                switch (StatusCode)
                {
                    case Const._StatusCode_Accepted:
                        Console.WriteLine("Exep.ExitCode: {0}", Exep.ExitCode);
                        if (ExitCodeDictionary.ContainsKey(Exep.ExitCode))
                        {
                            StatusCode = Const._StatusCode_RuntimeError;
                            Console.WriteLine("Exep.Exception: {0}", ExitCodeDictionary[Exep.ExitCode]);
                            Console.WriteLine("Exep.StandardError: {0}", Exep.StandardError.ReadToEnd());
                        }
                        break;
                    case Const._StatusCode_MemoryLimitExceeded:
                        UsedMemory = MemoryLimit;
                        break;
                    case Const._StatusCode_TimeLimitExceeded:
                        UsedTime = TimeLimit;
                        break;
                    default:
                        throw new Exception();
                }
            }

            return StatusCode;
        }

        private static void Watch()
        {
            while (!Exep.HasExited)
            {
                try
                {
                    UsedTime = Convert.ToInt64(Exep.TotalProcessorTime.TotalMilliseconds);
                    UsedMemory = Convert.ToInt64(Exep.PeakWorkingSet64);
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                if (Exep.TotalProcessorTime.TotalMilliseconds > TimeLimit)
                {
                    if (!Exep.HasExited) Exep.Kill();
                    StatusCode = Const._StatusCode_TimeLimitExceeded;
                    return;
                }
            }
        }

    }
}
