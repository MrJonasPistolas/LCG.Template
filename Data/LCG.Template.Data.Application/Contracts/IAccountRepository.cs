using LCG.Template.Common.Models.Account;
using System.Linq;

namespace LCG.Template.Data.Application.Contracts
{
    public interface IAccountRepository
    {
        IQueryable<AccountDetailsModel> GetAccountsByUserName(string username);
    }
}
