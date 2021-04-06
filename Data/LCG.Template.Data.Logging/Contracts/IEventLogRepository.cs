using LCG.Template.Common.Entities.Logging;
using System.Linq;

namespace LCG.Template.Data.Logging.Contracts
{
    public interface IEventLogRepository
    {
        bool Add(EventLog log);
        IQueryable<EventLog> Get();
    }
}
