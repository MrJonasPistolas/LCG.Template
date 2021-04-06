using LCG.Template.Common.Entities.Logging;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LCG.Template.Data.Logging.Repositories
{
    public class EventLogRepository
    {
        private readonly string _connection;
        private static readonly string _addEventLogInsertCmd;

        static EventLogRepository()
        {
            _addEventLogInsertCmd = Resources.SQLCommands.SQLCommands.AddEventLogInsertCommand;
        }

        public EventLogRepository(string connection)
        {
            _connection = connection;
        }

        public bool Add(EventLog log)
        {

            List<SqlParameter> paramList = new List<SqlParameter>();
            paramList.Add(new SqlParameter("EventID", log.EventId));
            paramList.Add(new SqlParameter("LogLevelId", log.LogLevelId));
            paramList.Add(new SqlParameter("LogLevel", log.LogLevel));
            paramList.Add(new SqlParameter("Message", log.Message));
            paramList.Add(new SqlParameter("CategoryName", log.CategoryName));
            paramList.Add(new SqlParameter("SystemLog", log.SystemLog));

            if (log.StackTrace != null)
                paramList.Add(new SqlParameter("StackTrace", log.StackTrace));
            else
                paramList.Add(new SqlParameter("StackTrace", DBNull.Value));

            if (log.AccountId != null)
                paramList.Add(new SqlParameter("AccountId", log.AccountId));
            else
                paramList.Add(new SqlParameter("AccountId", DBNull.Value));

            var result = ExecuteNonQuery(_addEventLogInsertCmd, paramList);
            if (result)
                return true;
            else
            {
                SaveLogToFile(paramList);
                return false;
            }
        }

        private bool ExecuteNonQuery(string commandStr, List<SqlParameter> paramList)
        {
            bool result = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connection))
                {
                    using (SqlCommand command = new SqlCommand(commandStr, conn))
                    {
                        command.Parameters.AddRange(paramList.ToArray());
                        if (conn.State != System.Data.ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        int count = command.ExecuteNonQuery();
                        conn.Close();
                        result = count > 0;
                    }
                }
            }
            catch { }

            return result;
        }

        private void SaveLogToFile(List<SqlParameter> paramList)
        {
            var logObj = paramList.Select(c => new { Name = c.ParameterName, c.Value });
            try
            {
                var path = Resources.LogMessages.LogMessages.LogFile;
                using (var writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(logObj));
                }
            }
            catch
            {
                //Logging to event viewer if everything goes wrong
                using (System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog("Application"))
                {
                    eventLog.Source = "LCG Application";
                    eventLog.WriteEntry(JsonConvert.SerializeObject(logObj), System.Diagnostics.EventLogEntryType.Error);
                }
            }
        }
    }
}
