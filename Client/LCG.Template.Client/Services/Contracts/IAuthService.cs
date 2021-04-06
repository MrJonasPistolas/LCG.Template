using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Models.Auth;
using System.Threading.Tasks;

namespace LCG.Template.Client.Services.Contracts
{
    public interface IAuthService
    {
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
    }
}
