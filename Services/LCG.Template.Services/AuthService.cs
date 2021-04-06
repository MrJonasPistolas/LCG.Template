using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Enums.Auth;
using LCG.Template.Common.Enums.Entities;
using LCG.Template.Common.Extensions.Session;
using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Models.Auth;
using LCG.Template.Common.Models.Extensions;
using LCG.Template.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LCG.Template.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly ILanguageService _languageService;
        private readonly ICypherService _cypherService;

        public AuthService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserService userService,
            IAccountService accountService,
            ILanguageService languageService,
            ICypherService cypherService)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _userService = userService;
            _accountService = accountService;
            _languageService = languageService;
            _cypherService = cypherService;
        }

        public async Task<LoginResult> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, loginModel.RememberMe, false);

                if (!result.Succeeded) return new LoginResult { Successful = false, Error = "Username and password are invalid.", Status = LoginStatus.Failed };

                var user = _userManager.Users.SingleOrDefault(u => u.UserName == loginModel.Username);
                var roles = await _userManager.GetRolesAsync(user);
                var accounts = await _accountService.GetAccountsByUserNameAsync(user);
                var accountUsers = await _userService.GetAccountUsersAsync(user.ApplicationUserId);
                var userInfo = await _userService.GetCurrentUserInformationAsync(user.ApplicationUserId);

                var languages = _languageService.GetLanguageByAccount(roles.Contains(SecurityRoles.Admin.ToString()) ? default(int?) : accounts.First().Id);

                var sessionInfo = new SessionAccountModel(user, roles, accounts.ToList(), userInfo, languages.ToList(), accountUsers);

                LoginStatus loginStatus = LoginStatus.PickAccount;

                if (accountUsers.Count == 1)
                {
                    //accountInfo.UserInfo.AccountUserId = accountUsers.FirstOrDefault().AccountUserId;
                    //await _httpContextAccessor.HttpContext.Session.AccountInformationAsync(accountInfo);
                    loginStatus = LoginStatus.Success;
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, loginModel.Username),
                    new Claim(ClaimTypes.UserData, _cypherService.Encrypt(JsonConvert.SerializeObject(sessionInfo)))
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtExpiryInDays"]));

                var token = new JwtSecurityToken(
                    _configuration["JwtIssuer"],
                    _configuration["JwtAudience"],
                    claims,
                    expires: expiry,
                    signingCredentials: creds
                );

                return new LoginResult { Successful = true, Token = new JwtSecurityTokenHandler().WriteToken(token), Status = loginStatus };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
