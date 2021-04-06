using LCG.Template.Common.Entities.Application;
using LCG.Template.Data.Application.Contracts;

namespace LCG.Template.Data.Application.Repositories
{
    public class AccountUsersRepository : RepositoryBase<AccountUser>, IAccountUsersRepository
    {
        public AccountUsersRepository(ApplicationDbContext context) : base(context)
        {
        }


    }
}
