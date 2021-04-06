using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Models.User;
using LCG.Template.Common.Tools.GridPagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LCG.Template.ServiceContracts
{
    public interface IAccountService
    {
        public Task<IEnumerable<AccountDetailsModel>> GetAccountsByUserNameAsync(ApplicationUser user);
        public IEnumerable<AccountDetailsModel> GetAccountsList(ref GridOptionsModel data);
        public IEnumerable<UserDetailsModel> GetAccountUsersList(int tennantAccountId, ref GridOptionsModel data);
        public Task ChangeUserDetailsAsync(int tennantAccountId, UserDetailsModel userDetailsModel);
        public Task<RoleAccountModel> CreateCustomRoleAsync(string roleName);
        public IEnumerable<RoleAccountModel> GetRoles();
        public IEnumerable<RoleAccountModel> GetTennantRoles();
        public Task<bool> TryInviteNewAccountUserAsync(AccountUserInviteModel model, int selectedAccountId);
        public Task<bool> DeleteUserFromAccountAsync(int userId, int selectedAccountId);
        public Task<bool> ReAddUserFromAccount(int userId, int selectedAccountId);
        public Task<bool> DeleteAccountAsync(int accountId);
        public Task<bool> ReAddAccountAsync(int accountId);
        public Task<bool> UpdateAccountDetailsAsync(AccountDetailsModel model);
        Task<bool> DisableUser(int userId);
        Task<bool> EnableUser(int userId);
        Task InviteNewAccountAsync(InviteNewAccountModel model);
    }
}
