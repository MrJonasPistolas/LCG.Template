using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Enums.Auth;
using LCG.Template.Common.Enums.Entities;
using LCG.Template.Common.Extensions.Session;
using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Models.Extensions;
using LCG.Template.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace LCG.Template.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly ILanguageService _languageService;

        public SessionService(IHttpContextAccessor httpContextAccessor, SignInManager<ApplicationUser> signInManager, IUserService userService, IAccountService accountService, ILanguageService languageService, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userService = userService;
            _accountService = accountService;
            _userManager = userManager;
            _languageService = languageService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SetSessionInfoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var accounts = await _accountService.GetAccountsByUserNameAsync(user);
            var accountUsers = await _userService.GetAccountUsersAsync(user.ApplicationUserId);
            var userInfo = await _userService.GetCurrentUserInformationAsync(user.ApplicationUserId);

            var languages = _languageService.GetLanguageByAccount(roles.Contains(SecurityRoles.Admin.ToString()) ? default(int?) : accounts.First().Id);

            var sessionInfo = new SessionAccountModel(user, roles, accounts.ToList(), userInfo, languages.ToList(), accountUsers);
            await _httpContextAccessor.HttpContext.Session.AccountInformationAsync(sessionInfo);
        }

        public async Task<LoginStatus> LoginAndSetSessionInfoAsync(LoginModel loginModel)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(loginModel.Username,
                   loginModel.Password, loginModel.RememberMe, false);

                if (result.Succeeded)
                {
                    var userToSignIn = _userManager.Users.SingleOrDefault(u => u.UserName == loginModel.Username);
                    await this.SetSessionInfoAsync(userToSignIn);
                    var accountInfo = await _httpContextAccessor.HttpContext.Session.AccountInformationAsync();
                    var accountUsers = await _userService.GetAccountUsersAsync(accountInfo.User.ApplicationUserId);

                    /*
                     * if the user has only one tennant
                     * then set account info automatically
                     * and the login process should have finished
                     */
                    if (accountUsers.Count == 1)
                    {
                        accountInfo.UserInfo.AccountUserId = accountUsers.FirstOrDefault().AccountUserId;
                        await _httpContextAccessor.HttpContext.Session.AccountInformationAsync(accountInfo);
                        return LoginStatus.Success;
                    }
                    return LoginStatus.PickAccount;
                }
                return LoginStatus.Failed;
            }
            catch (System.Exception ex)
            {
                throw;
            }
            
        }

        public async Task<bool> SetAccountInfo(int accountId)
        {
            var accountInfo = await _httpContextAccessor.HttpContext.Session.AccountInformationAsync();
            var accountUsers = await _userService.GetAccountUsersAsync(accountInfo.User.ApplicationUserId);
            if (accountInfo.IsMemberOf(accountId))
            {
                accountInfo.UserInfo.AccountUserId = accountUsers.FirstOrDefault(x => x.AccountId == accountId).AccountUserId;
                await _httpContextAccessor.HttpContext.Session.AccountInformationAsync(accountInfo);
                return true;
            }
            return false;
        }
    }
}
