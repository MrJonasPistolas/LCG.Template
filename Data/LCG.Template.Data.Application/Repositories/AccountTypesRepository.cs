using LCG.Template.Common.Entities.Application;
using LCG.Template.Common.Models.Account;
using LCG.Template.Data.Application.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LCG.Template.Data.Application.Repositories
{
    public class AccountTypesRepository : RepositoryBase<AccountUserType>, IAccountTypesRepository
    {
        public AccountTypesRepository(ApplicationDbContext context) : base(context)
        {
        }

        public AccountTypeModel CreateAccountType(string description)
        {
            var addedAccountType = Add(new AccountUserType
            {
                Active = true,
                Description = description
            });

            return new AccountTypeModel { 
                Description = addedAccountType.Description,
                Active = addedAccountType.Active,
                Id = addedAccountType.Id
            };
        }

        public async Task<AccountTypeModel> GetAccountTypeModelByDescriptionAsync(string description)
        {
            return await Get(x => x.Description == description).Select(x => new AccountTypeModel { 
                Id = x.Id,
                Description = x.Description,
                Active = x.Active
            }).FirstOrDefaultAsync();
        }
    }
}
