using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Enums;
using LCG.Template.Common.Enums.Applications;
using LCG.Template.Common.Enums.Entities;
using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Models.Application;
using LCG.Template.Common.Models.User;
using System.Collections.Generic;
using System.Linq;

namespace LCG.Template.Common.Models.Extensions
{
    public class SessionAccountModel
    {
        public int SelectedAccountId { get; set; }
        public int SelectedLanguageId { get; set; }
        public ApplicationUser User { get; set; }
        public UserDetailsModel UserInfo { get; set; }


        public IList<string> Roles { get; set; }
        public List<LanguageViewModel> Languages { get; set; }
        public List<AccountDetailsModel> UserAccounts { get; set; }


        public SessionAccountModel()
        {
        }
        public SessionAccountModel(ApplicationUser user, IList<string> roles, List<AccountDetailsModel> userAccounts, UserDetailsModel userInfo, List<LanguageViewModel> languages, List<AccountUsersModel> accountUsers)
        {
            this.User = user;
            this.Roles = roles;
            this.UserAccounts = userAccounts;
            this.SelectedAccountId = UserAccounts.First().Id;
            this.UserInfo = userInfo;
            this.Languages = languages;
            if (languages != null)
                this.SelectedLanguageId = languages.First().Id;
            if (accountUsers.Any())
            {
                this.UserInfo.AccountUserId = accountUsers.FirstOrDefault(x => x.AccountId == this.SelectedAccountId).AccountUserId;
            }
        }


        public bool IsDebug()
        {
#if DEBUG 
            return true;

#else
             return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
#endif

        }
        public AccountDetailsModel SelectedAccount()
        {
            return this.UserAccounts.FirstOrDefault(x => x.Id == this.SelectedAccountId);
        }
        public bool HasMultipleAccounts()
        {
            return !this.HasRole(ApplicationRoles.Admin) && UserAccounts.Count > 1;
        }
        public bool HasRole(string role)
        {
            return this.Roles.Contains(role);
        }
        public bool HasAccountType(SecurityRoles role)
        {
            return this.UserAccounts.Where(x => x.Type == StringEnum.GetEnumValue<string, Description>(role) && x.Id == this.SelectedAccountId).Count() > 0 ? true : false;
        }
        public bool HasRole(SecurityRoles role)
        {
            return this.Roles.Any(x => x.Equals(StringEnum.GetEnumValue<string, Description>(role)));
        }
        public bool IsMemberOf(int accountId)
        {
            var userid = User.ApplicationUserId;
            return UserAccounts.Any(c => c.Id == accountId);
        }

        public LanguageViewModel SelectedLanguage()
        {
            return this.Languages.FirstOrDefault(x => x.Id == this.SelectedLanguageId);
        }

        public LanguageViewModel SelectedLanguage(int id)
        {
            SelectedLanguageId = id;
            return SelectedLanguage();
        }
    }
}
