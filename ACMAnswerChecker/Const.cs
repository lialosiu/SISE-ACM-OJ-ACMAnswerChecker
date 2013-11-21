using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACMAnswerChecker
{
    static class Const
    {
        public const int _StatusCode_SystemError = -1;
        public const int _StatusCode_UnknownStatus = 0;
        public const int _StatusCode_Pending = 1;
        public const int _StatusCode_Compiling = 2;
        public const int _StatusCode_Running = 3;
        public const int _StatusCode_Accepted = 4;
        public const int _StatusCode_PresentationError = 5;
        public const int _StatusCode_WrongAnswer = 6;
        public const int _StatusCode_TimeLimitExceeded = 7;
        public const int _StatusCode_MemoryLimitExceeded = 8;
        public const int _StatusCode_OutputLimitExceeded = 9;
        public const int _StatusCode_RuntimeError = 10;
        public const int _StatusCode_CompileError = 11;

        public const int _LanguageCode_UnknownLanguage = 0;
        public const int _LanguageCode_C = 1;
        public const int _LanguageCode_CPP = 2;
        public const int _LanguageCode_Java = 3;

    }
}
