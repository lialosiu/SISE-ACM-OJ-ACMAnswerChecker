using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ACMAnswerChecker
{
    class Checker
    {
        private int _StatusCode;
        public int StatusCode
        {
            get
            {
                return this._StatusCode;
            }
        }

        public bool Check(string stdOutputData, string outputData)
        {
            string StdData = stdOutputData.Replace("\r", "");
            string Data = outputData.Replace("\r", "");

            string StdData_NP = StdData.Replace("\n", "").Replace("\t", "").Replace(" ", "");
            string Data_NP = Data.Replace("\n", "").Replace("\t", "").Replace(" ", "");

            if (StdData.Length < Data.Length / 2)
            {
                this._StatusCode = Const._StatusCode_OutputLimitExceeded;
                return false;
            }
            else if (StdData == Data)
            {
                this._StatusCode = Const._StatusCode_Accepted;
                return true;
            }
            else if (StdData_NP == Data_NP)
            {
                this._StatusCode = Const._StatusCode_PresentationError;
                return false;
            }
            else
            {
                this._StatusCode = Const._StatusCode_WrongAnswer;
                return false;
            }
        }
    }
}
