using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Context;
using System;
using System.Data.SqlClient;
using System.Globalization;

namespace PalestreGoGo.DataAccess
{
    public abstract class BaseRepository
    {
        //private readonly Regex REGEX_LOG_MESSAGE = new Regex(@"^([0-9]+)|([a-z0-9\:\.-]+)|([a-z])|([^|]+)", RegexOptions.IgnoreCase);
        protected readonly IConfiguration _configuration;

        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection GetConnection()
        {
            SqlConnection cn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            cn.InfoMessage += OnConnectionInfoMessage;
            return cn;
        }

        protected virtual void OnConnectionInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            var logMessage = TryParseMessage(e.Message);
            using (var logPropIdCliente = LogContext.PushProperty("IdCliente", logMessage.IdCliente))
                using(var logPropUserId = LogContext.PushProperty("UserId", logMessage.UserId))
            {
                switch (logMessage.Level)
                {
                    case LogMessageLevel.Unknown:
                    case LogMessageLevel.Info:
                        Log.Logger.Information(logMessage.Message);
                        break;
                    case LogMessageLevel.Error:
                        Log.Logger.Error(logMessage.Message);
                        break;
                    case LogMessageLevel.Warning:
                        Log.Logger.Warning(logMessage.Message);
                        break;
                    case LogMessageLevel.Verbose:
                        Log.Logger.Verbose(logMessage.Message);
                        break;
                }
            }
        }

        protected DataAccessLogMessage TryParseMessage(string msg)
        {
            DataAccessLogMessage result = null;
            try
            {
                string[] parts = msg.Split(new string[] { "||" }, StringSplitOptions.None);
                if (parts.Length == 4)
                {
                    result = new DataAccessLogMessage()
                    {
                        IdCliente = parts[0].Trim().Length > 0 ? int.Parse(parts[0]) : default(int?),
                        UserId = parts[1],
                        Level = parts[2].Trim().Length > 0 ? (LogMessageLevel)parts[2][0] : LogMessageLevel.Unknown,
                        DataMessaggio = parts[3].Trim().Length > 0 ? DateTime.Parse(parts[3], null, DateTimeStyles.RoundtripKind) : default(DateTime?),
                        Message = parts[4]
                    };
                }
                else
                {
                    result = new DataAccessLogMessage()
                    {
                        IdCliente = null,
                        Level = LogMessageLevel.Unknown,
                        UserId = null,
                        Message = msg
                    };
                }
            }
            catch
            {
                result = new DataAccessLogMessage()
                {
                    IdCliente = null,
                    Level = LogMessageLevel.Unknown,
                    UserId = null,
                    Message = msg
                };
            }
            return result;
        }
    }
}
