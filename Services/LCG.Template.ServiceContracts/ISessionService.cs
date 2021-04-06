using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Enums.Auth;
using LCG.Template.Common.Models.Account;
using System.Threading.Tasks;

namespace LCG.Template.ServiceContracts
{
    public interface ISessionService
    {
        public Task SetSessionInfoAsync(ApplicationUser user);
        public Task<LoginStatus> LoginAndSetSessionInfoAsync(LoginModel loginModel);
        public Task<bool> SetAccountInfo(int accountId);
    }
}
