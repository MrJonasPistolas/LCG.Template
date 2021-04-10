using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Enums.Auth;
using LCG.Template.Common.Enums.Entities;
using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Models.Auth;
using LCG.Template.Common.Models.Extensions;
using LCG.Template.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

                var accountUsers = await _userService.GetAccountUsersAsync(user.ApplicationUserId);

                LoginStatus loginStatus = LoginStatus.PickAccount;

                if (accountUsers.Count == 1)
                {
                    loginStatus = LoginStatus.Success;
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginModel.Username)
                };

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                claims.Add(new Claim("selected-account-user", accountUsers.FirstOrDefault().AccountUserId.ToString()));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var encryptingCredentials = new EncryptingCredentials(key, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512);
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
