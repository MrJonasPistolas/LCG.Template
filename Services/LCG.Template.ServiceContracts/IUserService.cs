using LCG.Template.Common.Models.Application;
using LCG.Template.Common.Models.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LCG.Template.ServiceContracts
{
    public interface IUserService
    {
        public Task<UserDetailsModel> GetUserDetailsAsync(int userId);
        public Task UpdateUserDetailsAsync(UserDetailsModel userDetailsModel);
        public Task<List<AccountUsersModel>> GetAccountUsersAsync(int applicationUserId);
        public Task<UserDetailsModel> GetCurrentUserInformationAsync(int applicationUserId);
        public Task ConfirmAndUpdateUser(ConfirmNewUserInputModel model);
        Task ForgotPasswordRequestAsync(string email);
        Task ResetPasswordAsync(PasswordResetInputModel passwordResetInputModel);
        Task UpdateLayoutPreferencesAsync(int userId, LayoutPreferencesModel preferencesModel);
        Task<LayoutPreferencesModel> GetLayoutPreferencesAsync(int userId);
        Task AskForChangePasswordAsync(string email);
    }
}
