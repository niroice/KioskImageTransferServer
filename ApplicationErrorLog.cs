using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace listenerTest
{
    

    static class ApplicationErrorLog
    {
        private static List<ApplicationError> errorLog = new List<ApplicationError>();

        public static void logError( ApplicationError error)
        {
            // add error to log list
            errorLog.Add(error);
        }

        public static List<ApplicationError> getErrorLogs()
        {
            return errorLog;
        }

    }
}

