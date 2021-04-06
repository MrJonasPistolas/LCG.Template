using LCG.Template.Common.Entities.Application;
using LCG.Template.Data.Application.Contracts;
using System.Collections.Generic;
using System.Linq;
using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Enums;
using LCG.Template.Common.Enums.Entities;

namespace LCG.Template.Data.Application.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public IQueryable<AccountDetailsModel> GetAccountsByUserName(string username)
        {
            var accounts = from account in Context.AccountsSet
                           join accountUsers in Context.AccountUsersSet on account.Id equals accountUsers.AccountId
                           join users in Context.UserSet on accountUsers.UserId equals users.Id
                           join accountType in Context.AccountUserTypeSet on accountUsers.AccountUserTypeId equals accountType.Id
                           where users.Email == username && account.Active && users.Active
                           orderby accountUsers.AccountId
                           select new AccountDetailsModel
                           {
                               Id = account.Id,
                               Name = account.Name,
                               AccountOwner = (
                                    from accountInner in Context.AccountsSet
                                    join accountUsersInner in Context.AccountUsersSet on account.Id equals accountUsersInner.AccountId
                                    join users in Context.UserSet on accountUsersInner.UserId equals users.Id
                                    join accountUserTypes in Context.AccountUserTypeSet on accountUsersInner.AccountUserTypeId equals accountUserTypes.Id
                                    where accountInner.Id == account.Id && accountUserTypes.Description == StringEnum.GetEnumValue<string, Description>(SecurityRoles.Tennant)
                                    select users.Email
                                    ).FirstOrDefault(),
                               TypeId = accountType.Id,
                               Type = accountType.Description
                           };

            return accounts;
        }
    }
}
