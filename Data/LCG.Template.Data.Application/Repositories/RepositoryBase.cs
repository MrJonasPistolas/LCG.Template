using LCG.Template.Common.Data.Contracts;
using LCG.Template.Common.Data.Repositories;

namespace LCG.Template.Data.Application.Repositories
{
    public abstract class RepositoryBase<T> : DataRepositoryBase<T, ApplicationDbContext>
        where T : class, IIdentifiableEntity, new()
    {
        public RepositoryBase(ApplicationDbContext context) : base(context)
        {

        }
    }
}
