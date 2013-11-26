using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACMAnswerChecker
{
    static class Const
    {
        public const short _StatusCode_SystemError = -1;
        public const short _StatusCode_UnknownStatus = 0;
        public const short _StatusCode_Pending = 1;
        public const short _StatusCode_Compiling = 2;
        public const short _StatusCode_Running = 3;
        public const short _StatusCode_Accepted = 4;
        public const short _StatusCode_PresentationError = 5;
        public const short _StatusCode_WrongAnswer = 6;
        public const short _StatusCode_TimeLimitExceeded = 7;
        public const short _StatusCode_MemoryLimitExceeded = 8;
        public const short _StatusCode_OutputLimitExceeded = 9;
        public const short _StatusCode_RuntimeError = 10;
        public const short _StatusCode_CompileError = 11;

        public const short _LanguageCode_UnknownLanguage = 0;
        public const short _LanguageCode_C = 1;
        public const short _LanguageCode_CPP = 2;
        public const short _LanguageCode_Java = 3;

    }
}
