using LCG.Template.Common.Enums.Auth;
using LCG.Template.Common.Models.Account;
using LCG.Template.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LCG.Template.App.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public LoginController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpPost]
        public async Task<LoginStatus> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var loginStatus = await _sessionService.LoginAndSetSessionInfoAsync(loginModel);
                return loginStatus;
            }
            return LoginStatus.Failed;
        }
    }
}
