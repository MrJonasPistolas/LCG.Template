using LCG.Template.Common.Data.Contracts;
using LCG.Template.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LCG.Template.Data.Logging.Repositories
{
    public abstract class RepositoryBase<T> : DataRepositoryBase<T, DbContext>
        where T : class, IIdentifiableEntity, new()
    {
        public RepositoryBase(DbContext context) : base(context)
        {

        }
    }
}
