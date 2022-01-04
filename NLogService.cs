using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace RegisterParcelsFromPC
{
    class NLogService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void PrintInfoLog(string str)
        {
            logger.Info(str);
        }
    }
}
