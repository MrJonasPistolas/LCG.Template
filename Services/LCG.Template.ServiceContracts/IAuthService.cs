using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Models.Auth;
using System.Threading.Tasks;

namespace LCG.Template.ServiceContracts
{
    public interface IAuthService
    {
        public Task<LoginResult> LoginAsync(LoginModel loginModel);
    }
}
