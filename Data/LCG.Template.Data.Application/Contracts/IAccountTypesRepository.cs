using LCG.Template.Common.Models.Account;
using System.Threading.Tasks;

namespace LCG.Template.Data.Application.Contracts
{
    public interface IAccountTypesRepository
    {
        Task<AccountTypeModel> GetAccountTypeModelByDescriptionAsync(string description);
        AccountTypeModel CreateAccountType(string description);
    }
}
