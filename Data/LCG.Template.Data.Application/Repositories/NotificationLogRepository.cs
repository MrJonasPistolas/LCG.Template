using LCG.Template.Common.Entities.Logging;
using LCG.Template.Data.Application.Contracts;

namespace LCG.Template.Data.Application.Repositories
{
    public class NotificationLogRepository : RepositoryBase<NotificationLog>, INotificationLogRepository
    {
        public NotificationLogRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
