using LCG.Template.Common.Data.Models;
using LCG.Template.Common.Entities.Application;
using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Enums.Entities;
using LCG.Template.Common.Exceptions.Application;
using LCG.Template.Common.Models.Account;
using LCG.Template.Common.Models.User;
using LCG.Template.Common.Tools.GridPagination;
using LCG.Template.Data.Application;
using LCG.Template.Data.Application.Repositories;
using LCG.Template.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCG.Template.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AccountService(IEmailService emailService, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _emailService = emailService;
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> TryInviteNewAccountUserAsync(AccountUserInviteModel model, int selectedAccountId)
        {
            var role = await _roleManager.FindByNameAsync(model.Role);

            if (role == null)
            {
                return false;
            }

            var userCreation = await _userManager.CreateAsync(new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email
            });

            var newUser = await _userManager.FindByEmailAsync(model.Email);

            if (userCreation.Succeeded)
            {
                var addingUserToRole = await _userManager.AddToRoleAsync(newUser, model.Role);
                if (!addingUserToRole.Succeeded)
                    throw new UserCreationException($"UserManager AddToRoleAsync failed: {string.Join("\n", addingUserToRole.Errors)}");

                using (var userRepository = new UserRepository(_applicationDbContext))
                {
                    var createdUser = userRepository.Add(new Common.Entities.Application.User
                    {
                        Id = newUser.ApplicationUserId,
                        Email = newUser.UserName
                    });

                    AccountUser accountUser = new AccountUser()
                    {
                        AccountId = selectedAccountId,
                        UserId = createdUser.Id,
                        AccountUserTypeId = (int)AccountUserTypes.User,
                    };

                    AccountUsersRepository accountUserRepository = new AccountUsersRepository(_applicationDbContext);

                    accountUserRepository.Add(accountUser);
                }

                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                _emailService.SendInvitedUserTemplate(1, newUser.UserName, null, emailConfirmationToken);

                return true;
            }
            else if (newUser != null)
            {
                using (var accountUserRepository = new AccountUsersRepository(_applicationDbContext))
                {
                    var userExistsInAccount = accountUserRepository.Get(x => x.AccountId == selectedAccountId && x.UserId == newUser.ApplicationUserId).Any();
                    if (!userExistsInAccount)
                    {
                        AccountUser accountUser = new AccountUser()
                        {
                            AccountId = selectedAccountId,
                            UserId = newUser.ApplicationUserId,
                            AccountUserTypeId = (int)AccountUserTypes.User,
                        };
                        accountUserRepository.Add(accountUser);
                    }
                }
                return true;
            }

            throw new UserCreationException($"UserManager CreateAsync failed: {string.Join("\n", userCreation.Errors)}");
        }

        public async Task<IEnumerable<AccountDetailsModel>> GetAccountsByUserNameAsync(ApplicationUser user)
        {
            using (var repository = new AccountRepository(_applicationDbContext))
            {
                var accountList = repository.GetAccountsByUserName(user.Email);
                return await accountList.ToListAsync();
            }
        }

        public async Task<bool> UpdateAccountDetailsAsync(AccountDetailsModel model)
        {
            using (var accountsRepository = new AccountRepository(_applicationDbContext))
            {
                var account = accountsRepository.Get(x => x.Id == model.Id).FirstOrDefault();
                if (account != null)
                {
                    account.Email = model.Email;
                    account.Name = model.Name;
                    account.Nif = model.Nif;
                    account.PhoneNumber = model.PhoneNumber;
                    account.Website = model.Website;
                    account.ZipCode = model.ZipCode;
                    account.Address = model.Address;
                    account.City = model.City;
                    await accountsRepository.UpdateAsync(account);
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<AccountDetailsModel> GetAccountsList(ref GridOptionsModel data)
        {
            var pageOption = data.CastTo<PageOption>();

            using (var accountsRepository = new AccountRepository(_applicationDbContext))
            {
                var list = accountsRepository
                    .Get(ref pageOption)
                    .Include(x => x.AccountUsers)
                    .ThenInclude(x => x.User)
                    .Select(x => new AccountDetailsModel
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Name = x.Name,
                        Active = x.Active,
                        City = x.City,
                        Address = x.Address,
                        ZipCode = x.ZipCode,
                        Nif = x.Nif,
                        Website = x.Website,
                        PhoneNumber = x.PhoneNumber,
                        Users = x.AccountUsers.Select(y => new { y.User, y.Active }).Select(obj => new UserDetailsModel
                        {
                            Id = obj.User.Id,
                            Email = obj.User.Email,
                            ActiveInAccount = obj.Active,
                            Active = obj.User.Active,
                            CompanyName = obj.User.CompanyName,
                            FirstName = obj.User.FirstName,
                            LastName = obj.User.LastName,
                            MiddleNames = obj.User.MiddleNames,
                            PhoneNumber = obj.User.PhoneNumber,
                            TaxNumber = obj.User.TaxNumber,
                            ImageUrl = obj.User.ImageUrl,
                            LayoutPreferences = obj.User.LayoutPreferences,
                            AccountUserId = x.Id,
                            InviteState = _userManager.FindByEmailAsync(obj.User.Email).GetAwaiter().GetResult().EmailConfirmed ? AccountInviteStates.Confirmed : AccountInviteStates.Pending
                        })
                    });
                return list;
            }
        }

        public async Task<bool> DeleteAccountAsync(int accountId)
        {
            using (var accountsRepository = new AccountRepository(_applicationDbContext))
            {
                var account = accountsRepository.Get(x => x.Id == accountId).FirstOrDefault();
                if (account != null)
                {
                    account.Active = false;
                    await accountsRepository.UpdateAsync(account);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ReAddAccountAsync(int accountId)
        {
            using (var accountsRepository = new AccountRepository(_applicationDbContext))
            {
                var account = accountsRepository.Get(x => x.Id == accountId).FirstOrDefault();
                if (account != null)
                {
                    account.Active = true;
                    await accountsRepository.UpdateAsync(account);
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<UserDetailsModel> GetAccountUsersList(int tennantAccountId, ref GridOptionsModel data)
        {
            var pageOption = data.CastTo<PageOption>();

            using (var accountUsersRepository = new AccountUsersRepository(_applicationDbContext))
            {
                using (var usersRepository = new UserRepository(_applicationDbContext))
                {
                    var accUsers = accountUsersRepository.Get()
                        .Where(x => x.AccountId == tennantAccountId
                            && x.AccountUserTypeId == (int)AccountUserTypes.User);

                    var users =
                        usersRepository.Get(from user in usersRepository.Get()
                                            join accUsr in accUsers
                                            on user.Id equals accUsr.UserId
                                            select new UserDetailsModel
                                            {
                                                Id = user.Id,
                                                ActiveInAccount = accUsr.Active,
                                                Email = user.Email,
                                                CompanyName = user.CompanyName,
                                                FirstName = user.FirstName,
                                                LastName = user.LastName,
                                                MiddleNames = user.MiddleNames,
                                                PhoneNumber = user.PhoneNumber,
                                                TaxNumber = user.TaxNumber,
                                                ImageUrl = user.ImageUrl,
                                                LayoutPreferences = user.LayoutPreferences
                                            }, ref pageOption);
                    data.Count = pageOption.Count;

                    var userList = users.ToList();

                    foreach (var user in userList)
                    {
                        user.InviteState = _userManager.FindByEmailAsync(user.Email).GetAwaiter().GetResult().EmailConfirmed ? AccountInviteStates.Confirmed : AccountInviteStates.Pending;
                    }

                    return userList;
                }
            }
        }

        public async Task ChangeUserDetailsAsync(int tennantAccountId, UserDetailsModel userDetailsModel)
        {
            using (var accountUsersRepository = new AccountUsersRepository(_applicationDbContext))
            {
                using (var usersRepository = new UserRepository(_applicationDbContext))
                {
                    var userExistsAndBelongsToTennant = accountUsersRepository.Get()
                        .Where(x => x.AccountId == tennantAccountId
                            && x.AccountUserTypeId == (int)AccountUserTypes.User
                            && x.Active && x.UserId == userDetailsModel.Id).Any();

                    if (!userExistsAndBelongsToTennant)
                        return;

                    var getUser = usersRepository.Get(x => x.Id == userDetailsModel.Id).FirstOrDefault();

                    if (getUser == null)
                        return;

                    getUser.FirstName = userDetailsModel.FirstName != getUser.FirstName ? userDetailsModel.FirstName : getUser.FirstName;
                    getUser.CompanyName = userDetailsModel.CompanyName != getUser.CompanyName ? userDetailsModel.CompanyName : getUser.CompanyName;
                    getUser.LastName = userDetailsModel.LastName != getUser.LastName ? userDetailsModel.LastName : getUser.LastName;
                    getUser.TaxNumber = userDetailsModel.TaxNumber != getUser.TaxNumber ? userDetailsModel.TaxNumber : getUser.TaxNumber;
                    getUser.MiddleNames = userDetailsModel.MiddleNames != getUser.MiddleNames ? userDetailsModel.MiddleNames : getUser.MiddleNames;
                    getUser.PhoneNumber = userDetailsModel.PhoneNumber != getUser.PhoneNumber ? userDetailsModel.PhoneNumber : getUser.PhoneNumber;

                    await usersRepository.UpdateAsync(getUser);
                }
            }
        }

        public async Task<RoleAccountModel> CreateCustomRoleAsync(string roleName)
        {
            using (var accountTypeRepository = new AccountTypesRepository(_applicationDbContext))
            {
                var accountTypeModel = await accountTypeRepository.GetAccountTypeModelByDescriptionAsync(roleName);
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role != null && accountTypeModel != null)
                {
                    return new RoleAccountModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        AccountType = accountTypeModel
                    };
                }

                if (role == null && accountTypeModel == null)
                {
                    var identityRoleCreation = await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = roleName
                    });

                    if (identityRoleCreation.Succeeded)
                    {
                        var accountType = accountTypeRepository.CreateAccountType(roleName);

                        role = await _roleManager.FindByNameAsync(roleName);

                        return new RoleAccountModel
                        {
                            RoleId = role.Id,
                            RoleName = role.Name,
                            AccountType = accountType,
                        };
                    }
                    throw new RoleCreationException($"RoleManager CreateAsync failed: {string.Join("\n", identityRoleCreation.Errors)}");
                }

                throw new RoleCreationException("Something is seriously wrong with the DB. Role and AccountType aren't in sync!");
            }
        }

        public IEnumerable<RoleAccountModel> GetRoles()
        {
            return _roleManager.Roles.Select(x => new RoleAccountModel
            {
                RoleName = x.Name,
                RoleId = x.Id
            });
        }

        public IEnumerable<RoleAccountModel> GetTennantRoles()
        {
            var forbiddenTennantRoles = new string[] { "Admin", "Tennant" };
            return this.GetRoles().Where(x => !forbiddenTennantRoles.Contains(x.RoleName));
        }

        public async Task<bool> DeleteUserFromAccountAsync(int userId, int selectedAccountId)
        {
            using (var accountUsersRepository = new AccountUsersRepository(_applicationDbContext))
            {
                var user = accountUsersRepository.Get(x => x.AccountId == selectedAccountId && x.UserId == userId).FirstOrDefault();
                if (user != null)
                {
                    user.Active = false;
                    await accountUsersRepository.UpdateAsync(user);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ReAddUserFromAccount(int userId, int selectedAccountId)
        {
            using (var accountUsersRepository = new AccountUsersRepository(_applicationDbContext))
            {
                var user = accountUsersRepository.Get(x => x.AccountId == selectedAccountId && x.UserId == userId).FirstOrDefault();
                if (user != null)
                {
                    user.Active = true;
                    await accountUsersRepository.UpdateAsync(user);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DisableUser(int userId)
        {
            using (var userRepository = new UserRepository(_applicationDbContext))
            {
                var user = userRepository.Get(x => x.Id == userId).FirstOrDefault();
                if (user != null)
                {
                    user.Active = false;
                    await userRepository.UpdateAsync(user);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> EnableUser(int userId)
        {
            using (var userRepository = new UserRepository(_applicationDbContext))
            {
                var user = userRepository.Get(x => x.Id == userId).FirstOrDefault();
                if (user != null)
                {
                    user.Active = true;
                    await userRepository.UpdateAsync(user);
                    return true;
                }
            }
            return false;
        }

        public async Task InviteNewAccountAsync(InviteNewAccountModel model)
        {
            var role = await _roleManager.FindByNameAsync("Tennant");
            if (role == null)
            {
                throw new InvalidOperationException("Something went terribly wrong with InviteNewAccountAsync. Fatal.");
            }

            using (var userRepository = new UserRepository(_applicationDbContext))
            {
                var newUser = await _userManager.FindByEmailAsync(model.Email);
                var userAlreadyExisted = false;
                User user = null;
                if (newUser == null)
                {
                    var userCreation = await _userManager.CreateAsync(new ApplicationUser
                    {
                        Email = model.Email,
                        UserName = model.Email
                    });
                    if (userCreation.Succeeded)
                    {
                        newUser = await _userManager.FindByEmailAsync(model.Email);
                        user = userRepository.Add(new User
                        {
                            Active = true,
                            Email = model.Email,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            MiddleNames = model.MiddleNames,
                            PhoneNumber = model.PhoneNumber,
                            TaxNumber = model.TaxNumber,
                            CompanyName = model.CompanyName,
                            Id = newUser.ApplicationUserId
                        });
                    }
                    else
                        throw new UserCreationException($"UserManager CreateAsync failed: {string.Join("\n", userCreation.Errors)}");
                }
                else
                {
                    user = userRepository.Get(x => x.Email == model.Email).FirstOrDefault();
                    if (user == null)
                        throw new InvalidOperationException("Something went terribly wrong with InviteNewAccountAsync. Fatal.");
                    userAlreadyExisted = true;
                }

                
                var addingUserToRole = await _userManager.AddToRoleAsync(newUser, role.Name);

                using (var accountUserRepository = new AccountUsersRepository(_applicationDbContext))
                {
                    var accountAlreadyExists = accountUserRepository.Get().Include(x => x.User).Where(x => x.User.Email == model.Email).Any();
                    if (!accountAlreadyExists)
                    {
                        using (var accountRepository = new AccountRepository(_applicationDbContext))
                        {
                            var acc = accountRepository.Add(new Account { 
                                PhoneNumber = model.PhoneNumber,
                                Active = true,
                                Address = model.Address,
                                City = model.City,
                                Email = model.Email,
                                Name = model.AccountName,
                                Nif = model.TaxNumber,
                                ZipCode = model.ZipCode,
                                LanguageId = 1
                            });

                            accountUserRepository.Add(new AccountUser { 
                                AccountId = acc.Id,
                                UserId = user.Id,
                                Active = true,
                                AccountUserTypeId = (int)AccountUserTypes.Tennant
                            });

                            if (!userAlreadyExisted)
                            {
                                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                                _emailService.SendInvitedUserTemplate(1, newUser.UserName, null, emailConfirmationToken);
                            }
                        }
                    }
                }
            }
        }
    }
}

