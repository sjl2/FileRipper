using System;
using System.Collections.Generic;

namespace FileRipper
{
    public class LogWriter
    {
        // Private Constructor
        private LogWriter()
        {
            _logQueue = new List<Log>();
        }

        #region Private Properties
        private static LogWriter _instance; // Instance of Logwriter
        private static List<Log> _logQueue; // Queue
        #endregion Private Properties

        // Public "Constructor"
        public static LogWriter Instance()
        {
            return _instance ?? (_instance = new LogWriter());
        }
       
        // Method for Logging 
        public void LogFileRip(string status, string message, string server)
        {
            var vals = new Log {Status = status, Message = message, Server = server};
            //LogFileRip(vals);
            AddToQueue(vals);
        }

        // Adds to private queue 
        private void AddToQueue(Log log)
        {
            _logQueue.Add(log);
            Flush(false);
        }

        // Actually Logs to SQL Server
        private static void SendLog(Log log)
        {
            const string qs = @"INSERT INTO [JJFDEVELOPMENT].[SCM].[dbo].[FileRipLog] ([Status],[Message],[Server]) VALUES (@Status, @Message, @Server)";
            SqlMapperUtil.InsertUpdateOrDeleteSql(qs, log);
        }

        // Flush Queue 
        public void Flush(Boolean isEnd)
        {
            // Thousand Count limit 
            if (_logQueue.Count < 1000 && !isEnd) return;

            foreach (var log in _logQueue)
            {
                // Merges
                SendLog(log);
            }
            // Clears List
            _logQueue.Clear();
            
        }
    }
}
