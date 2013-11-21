using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ACMAnswerChecker
{
    class DBUpdater
    {
        MySqlConnection mysqlConnection = new MySqlConnection("Server=localhost;Database=sise_acm_oj;Uid=sise_acm_oj;Pwd=sise_acm_oj_sql");

        public void Update(Int64 answerID, Int64 problemID, int statusCode, string info, Int64 usedTime, Int64 usedMemory, string inputData, string outputData)
        {
            mysqlConnection.Open();
            try
            {
                MySqlCommand cmd = mysqlConnection.CreateCommand();
                MySqlCommand cmd2 = mysqlConnection.CreateCommand();

                switch (statusCode)
                {
                    case Const._StatusCode_Accepted:
                        //status = "Accepted";
                        cmd2.CommandText = "UPDATE `problem` SET `SubmitCount`=`SubmitCount`+1, `AcceptedCount`=`AcceptedCount`+1 WHERE `ID`=@ID";
                        break;
                    case Const._StatusCode_PresentationError:
                        //status = "Presentation Error";
                        cmd2.CommandText = "UPDATE `problem` SET `SubmitCount`=`SubmitCount`+1, `PresentationErrorCount`=`PresentationErrorCount`+1 WHERE `ID`=@ID";
                        break;
                    case Const._StatusCode_WrongAnswer:
                        //status = "Wrong Answer";
                        cmd2.CommandText = "UPDATE `problem` SET `SubmitCount`=`SubmitCount`+1, `WrongAnswerCount`=`WrongAnswerCount`+1 WHERE `ID`=@ID";
                        break;
                    case Const._StatusCode_TimeLimitExceeded:
                        //status = "Time Limit Exceeded";
                        cmd2.CommandText = "UPDATE `problem` SET `SubmitCount`=`SubmitCount`+1, `TimeLimitExceededCount`=`TimeLimitExceededCount`+1 WHERE `ID`=@ID";
                        break;
                    case Const._StatusCode_MemoryLimitExceeded:
                        //status = "Memory Limit Exceeded";
                        cmd2.CommandText = "UPDATE `problem` SET `SubmitCount`=`SubmitCount`+1, `MemoryLimitExceededCount`=`MemoryLimitExceededCount`+1 WHERE `ID`=@ID";
                        break;
                    case Const._StatusCode_OutputLimitExceeded:
                        //status = "Output Limit Exceeded";
                        cmd2.CommandText = "UPDATE `problem` SET `SubmitCount`=`SubmitCount`+1, `OutputLimitExceededCount`=`OutputLimitExceededCount`+1 WHERE `ID`=@ID";
                        break;
                    case Const._StatusCode_RuntimeError:
                        //status = "Runtime Error";
                        cmd2.CommandText = "UPDATE `problem` SET `SubmitCount`=`SubmitCount`+1, `RuntimeErrorCount`=`RuntimeErrorCount`+1 WHERE `ID`=@ID";
                        break;
                    case Const._StatusCode_CompileError:
                        //status = "Compile Error";
                        cmd2.CommandText = "UPDATE `problem` SET `SubmitCount`=`SubmitCount`+1, `CompileErrorCount`=`CompileErrorCount`+1 WHERE `ID`=@ID";
                        break;
                    case Const._StatusCode_Pending:
                        //status = "Pending";
                    case Const._StatusCode_SystemError:
                        //status = "System Error";
                    default:
                        //status = "Unkonwn Status";
                        cmd2.CommandText = "UPDATE `problem` SET `SubmitCount`=`SubmitCount`+1, `SystemErrorCount`=`SystemErrorCount`+1 WHERE `ID`=@ID";
                        break;
                }

                cmd.CommandText = "UPDATE `answer` SET `StatusCode`=@StatusCode, `InputData`=@InputData, `OutputData`=@OutputData, `UsedTime`=@UsedTime, `UsedMemory`=@UsedMemory WHERE `ID`=@ID";
                cmd.Parameters.AddWithValue("@ID", answerID);
                cmd.Parameters.AddWithValue("@InputData", inputData);
                cmd.Parameters.AddWithValue("@OutputData", outputData);
                cmd.Parameters.AddWithValue("@UsedTime", usedTime);
                cmd.Parameters.AddWithValue("@UsedMemory", usedMemory);
                cmd.Parameters.AddWithValue("@StatusCode", statusCode);
                cmd.ExecuteNonQuery();

                cmd2.Parameters.AddWithValue("@ID", problemID);
                cmd2.ExecuteNonQuery();

                mysqlConnection.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
