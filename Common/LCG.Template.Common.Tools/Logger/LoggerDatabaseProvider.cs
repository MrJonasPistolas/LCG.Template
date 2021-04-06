using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LCG.Template.Common.Tools.Logger
{
    public class LoggerDatabaseProvider : ILoggerProvider
    {
        private readonly string _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggerDatabaseProvider(string connectionString, IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = connectionString;
            _httpContextAccessor = httpContextAccessor;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new Logger(categoryName, _connectionString, _httpContextAccessor);
        }

        public void Dispose()
        {
        }
    }
}
