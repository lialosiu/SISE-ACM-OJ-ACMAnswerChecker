using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ACMAnswerChecker
{
    static class Checker
    {
        public static Int16 StatusCode { get; private set; }

        public static int Check(Answer thisAnswer, Problem thisProblem)
        {
            string stdData = thisProblem.StandardOutput.Replace("\r", "");
            string data = thisAnswer.OutputData.Replace("\r", "");

            string stdDataNp = stdData.Replace("\n", "").Replace("\t", "").Replace(" ", "");
            string dataNp = data.Replace("\n", "").Replace("\t", "").Replace(" ", "");

            if (stdData.Length < data.Length / 2)
            {
                StatusCode = Const._StatusCode_OutputLimitExceeded;
            }
            else if (stdData == data)
            {
                StatusCode = Const._StatusCode_Accepted;
            }
            else if (stdDataNp == dataNp)
            {
                StatusCode = Const._StatusCode_PresentationError;
            }
            else
            {
                StatusCode = Const._StatusCode_WrongAnswer;
            }

            return StatusCode;
        }
    }
}
