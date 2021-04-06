using LCG.Template.Common.Entities.Logging;
using LCG.Template.Common.Enums.Identity;
using LCG.Template.Data.Logging.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;

namespace LCG.Template.Common.Tools.Logger
{
    public class Logger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _connection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private HttpContext _httpContext => _httpContextAccessor.HttpContext;

        public Logger(string categoryName, string connection, IHttpContextAccessor httpContextAccessor)
        {
            _connection = connection;
            _categoryName = categoryName;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? AccountId
        {
            get
            {
                if (_httpContext != null
                    && _httpContext.Features.Get<ISessionFeature>() != null
                    && _session != null
                    && _session.IsAvailable)
                {
                    var accountId = _session.GetInt32(Enum.GetName(typeof(SessionExtensionKeys), SessionExtensionKeys.SelectedAccountId));
                    return accountId;
                }
                return null;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

            var log = new EventLog()
            {
                EventId = eventId.Id,
                LogLevelId = (int)logLevel,
                LogLevel = logLevel.ToString(),
                Message = formatter(state, exception),
                CategoryName = _categoryName,
                SystemLog = true
            };

            if (eventId.Id < 0)
            {
                log.LogLevel = LogLevel.Information.ToString();
                log.LogLevelId = (int)LogLevel.Information;
                log.AccountId = this.AccountId;
                log.SystemLog = false;
            }

            if (exception != null && exception.StackTrace != null)
                log.StackTrace = exception.StackTrace;

            InsertLog(log);
        }


        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        private bool InsertLog(EventLog log)
        {
            EventLogRepository repository = new EventLogRepository(_connection);
            return repository.Add(log);
        }
    }
}
