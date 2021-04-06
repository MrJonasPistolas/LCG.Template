using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Models.Application;
using LCG.Template.Common.Models.User;
using LCG.Template.Data.Application;
using LCG.Template.Data.Application.Repositories;
using LCG.Template.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCG.Template.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public UserService(ApplicationDbContext applicationDbContext, ILogger<UserService> logger, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _applicationDbContext = applicationDbContext;
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<UserDetailsModel> GetUserDetailsAsync(int userId)
        {
            using (var userRepository = new UserRepository(_applicationDbContext))
            {
                return (UserDetailsModel)await userRepository.GetUserByIdAsync(userId);
            }
        }

        public async Task UpdateUserDetailsAsync(UserDetailsModel userDetailsModel)
        {
            using (var userRepository = new UserRepository(_applicationDbContext))
            {
                var user = await userRepository.GetUserByIdAsync(userDetailsModel.Id);
                user.FirstName = userDetailsModel.FirstName;
                user.MiddleNames = userDetailsModel.MiddleNames;
                user.LastName = userDetailsModel.LastName;
                user.PhoneNumber = userDetailsModel.PhoneNumber;
                user.TaxNumber = userDetailsModel.TaxNumber;
                user.CompanyName = userDetailsModel.CompanyName;
                await userRepository.UpdateAsync(user);
            }
        }

        public async Task<List<AccountUsersModel>> GetAccountUsersAsync(int applicationUserId)
        {
            var accountUserRepository = new AccountUsersRepository(_applicationDbContext);

            var ret = await accountUserRepository.Get(x => x.UserId == applicationUserId && x.Active)
            .Select(x => new AccountUsersModel()
            {
                AccountId = x.AccountId,
                AccountUserId = x.Id
            }).ToListAsync();
            return ret;

        }

        public async Task<UserDetailsModel> GetCurrentUserInformationAsync(int applicationUserId)
        {
            using (var userRepository = new UserRepository(_applicationDbContext))
            {
                var userViewModel = await userRepository.Get(u => u.Id == applicationUserId)
                .Select(user => new UserDetailsModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    CompanyName = user.CompanyName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MiddleNames = user.MiddleNames,
                    PhoneNumber = user.PhoneNumber,
                    TaxNumber = user.TaxNumber,
                    ImageUrl = user.ImageUrl,
                    LayoutPreferences = user.LayoutPreferences,
                }).FirstOrDefaultAsync();
                return userViewModel;
            }
        }

        public async Task ConfirmAndUpdateUser(ConfirmNewUserInputModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            //TODO: confirm user null, if yes, throw exception
            var confirmSuccess = await _userManager.ConfirmEmailAsync(user, model.ConfirmationToken);
            var passwordUpdateSuccess = await _userManager.AddPasswordAsync(user, model.Password);
            if (confirmSuccess.Succeeded && passwordUpdateSuccess.Succeeded)
            {
                var userDetails = new UserDetailsModel {
                    Id = user.ApplicationUserId,
                    FirstName = model.FirstName,
                    MiddleNames = model.MiddleNames,
                    CompanyName = model.CompanyName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    TaxNumber = model.TaxNumber
                };
                await this.UpdateUserDetailsAsync(userDetails);
                return;
            }
            //TODO: create a specific exception for this
            throw new Exception("ConfirmAndUpdateUser failed.");
        }

        public async Task ForgotPasswordRequestAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return;
            var resetPasswordTkn = await _userManager.GeneratePasswordResetTokenAsync(user);
            _emailService.SendPasswordResetTemplate(1, email, null, resetPasswordTkn);
        }

        public async Task ResetPasswordAsync(PasswordResetInputModel passwordResetInputModel)
        {
            var user = await _userManager.FindByEmailAsync(passwordResetInputModel.Email);
            if (user == null)
                return;
            await _userManager.ResetPasswordAsync(user, passwordResetInputModel.ConfirmationToken, passwordResetInputModel.Password);
        }

        public async Task UpdateLayoutPreferencesAsync(int userId, LayoutPreferencesModel preferencesModel)
        {
            using (var userRepository = new UserRepository(_applicationDbContext))
            {
                var user = await userRepository.GetUserByIdAsync(userId);
                user.LayoutPreferences = JsonConvert.SerializeObject(preferencesModel);
                await userRepository.UpdateAsync(user);
            }
        }

        public async Task<LayoutPreferencesModel> GetLayoutPreferencesAsync(int userId)
        {
            using (var userRepository = new UserRepository(_applicationDbContext))
            {
                var user = await userRepository.GetUserByIdAsync(userId);
                return string.IsNullOrEmpty(user.LayoutPreferences) ? new LayoutPreferencesModel() : JsonConvert.DeserializeObject<LayoutPreferencesModel>(user.LayoutPreferences);
            }
        }

        public async Task AskForChangePasswordAsync(string email)
        {
            await this.ForgotPasswordRequestAsync(email);
        }
    }
}
