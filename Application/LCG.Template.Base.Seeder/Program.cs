using LCG.Template.Common.Entities.Application;
using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Enums;
using LCG.Template.Common.Enums.Entities;
using LCG.Template.Data.Application;
using LCG.Template.Data.Identity;
using LCG.Template.Data.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCG.Template.Base.Seeder
{
    static class JSONFiles
    {
        public static string PARENT_FOLDER = Path.Combine("Resources", "Json");
        public const string ACCOUNT_USER_TYPES = "account-user-types.json";
        public const string LANGUAGES = "languages.json";
    }

    static class SQLScripts
    {
    }

    static class ConnectionStrings
    {
        public const string APPLICATION = "Application";
        public const string SECURITY = "Security";
        public const string LOGGER = "Logger";
    }

    class Program
    {
        private static IConfiguration _configuration;
        private static ServiceProvider _services;
        private static bool _debugMode = false;
        private static bool _disableMigrations = false;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Change this seeder to meet your app needs.");
            Console.WriteLine("Starting...");
            try
            {
                Console.WriteLine("Running initial setup...");
                SetupConfig(args);
                SetupServices();
                Console.WriteLine("Seeding...");
                if (!_disableMigrations)
                    RunMigrations();
                await SetupSecurityRolesAsync();
                await SetupUserTypesAsync();
                await SetupLanguagesAsync();
                await CreateUsersAsync();
            }
            catch (Exception ex)
            {
                HandleGeneralException(ex);
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }



        private static void SetupConfig(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", true)
                .AddCommandLine(args)
                .Build();
            _disableMigrations = _configuration["DisableMigrations"] == "true";
            _debugMode = _configuration["DEBUG"] == "true";
#if DEBUG
            _debugMode = true;
#endif
        }


        #region Set up
        private static void SetupServices()
        {

            var identityConnection = _configuration.GetConnectionString(ConnectionStrings.SECURITY);
            var applicationConnection = _configuration.GetConnectionString(ConnectionStrings.APPLICATION);
            var loggerConnection = _configuration.GetConnectionString(ConnectionStrings.LOGGER);
            if (string.IsNullOrEmpty(identityConnection) || string.IsNullOrEmpty(applicationConnection) || string.IsNullOrEmpty(loggerConnection))
            {
                throw new Exception("Connection strings aren't setup properly.");
            }
            var services = new ServiceCollection()
                .AddDbContext<SecurityDbContext>(options => options.UseSqlServer(identityConnection))
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(applicationConnection))
                .AddDbContext<LoggerDbContext>(options => options.UseSqlServer(loggerConnection));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<SecurityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            services.AddLogging();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            _services = services.BuildServiceProvider();

        }

        private static void RunMigrations()
        {
            using (var context = new ApplicationDbContext(_configuration.GetConnectionString(ConnectionStrings.APPLICATION)))
            {
                context.Database.Migrate();
            }
            using (var context = new SecurityDbContext(_configuration.GetConnectionString(ConnectionStrings.SECURITY)))
            {
                context.Database.Migrate();
            }
            using (var context = new LoggerDbContext(_configuration.GetConnectionString(ConnectionStrings.LOGGER)))
            {
                context.Database.Migrate();
            }
        }
        #endregion

        #region Security DB Manipulation

        private static async Task SetupSecurityRolesAsync()
        {
            ShowSimpleInfo("SetupSecurityRolesAsync", "Setting up identity roles");
            using (var serviceScope = _services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                if (roleManager.Roles.Any())
                {
                    ShowSimpleInfo("SetupSecurityRolesAsync", "Roles table not empty", "Returning");
                    return;
                }
                #region Admin Role Creation

                var adminRole = new IdentityRole(StringEnum.GetEnumValue<string, Description>(SecurityRoles.Admin));
                await roleManager.CreateAsync(adminRole);

                #endregion

                #region Account Owner Role Creation

                var accountOwnerRole = new IdentityRole(StringEnum.GetEnumValue<string, Description>(SecurityRoles.Tennant));
                await roleManager.CreateAsync(accountOwnerRole);

                #endregion

                #region Account Final User Role Creation

                var accountFinalUserRole = new IdentityRole(StringEnum.GetEnumValue<string, Description>(SecurityRoles.User));
                await roleManager.CreateAsync(accountFinalUserRole);

                #endregion
            }
        }

        private static async Task CreateUsersAsync()
        {
            using (var serviceScope = _services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                UserManager<ApplicationUser> userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                using (SecurityDbContext securityContext = new SecurityDbContext(_configuration.GetConnectionString(ConnectionStrings.SECURITY)))
                {
                    using (ApplicationDbContext applicationContext = new ApplicationDbContext(_configuration.GetConnectionString(ConnectionStrings.APPLICATION)))
                    {
                        var appAdminUsers = GetJsonObject<User[]>("admin-users.json");
                        var adminUsers = GetJsonObject<ApplicationUser[]>("admin-users.json");

                        var appAccountOwnersUsers = GetJsonObject<User[]>("tennants.json");
                        var accountOwnersUsers = GetJsonObject<ApplicationUser[]>("tennants.json");

                        var appAccountFinalUsers = GetJsonObject<User[]>("users.json");
                        var accountFinalUsers = GetJsonObject<ApplicationUser[]>("users.json");

                        var accounts = GetJsonObject<Account[]>("accounts.json");

                        var accountUserTypes = GetJsonObject<AccountUserType[]>("account-user-types.json");

                        var appAccountsUsers = GetJsonObject<User[]>("accounts-users.json");
                        var accountsUsers = GetJsonObject<AccountUser[]>("accounts-users.json");

                        #region Security UserCreation

                        if (!securityContext.Users.Any())
                        {
                            foreach (var user in adminUsers)
                            {
                                var pass = user.UserName;
                                await userManager.CreateAsync(user, pass);
                                await userManager.AddToRoleAsync(user, StringEnum.GetEnumValue<string, Description>(SecurityRoles.Admin));

                                var appUser = new User
                                {
                                    Id = user.ApplicationUserId,
                                    Email = user.UserName,
                                    FirstName = appAdminUsers.First(x => x.Email == user.Email).FirstName,
                                    LastName = appAdminUsers.First(x => x.Email == user.Email).LastName,
                                    CompanyName = appAdminUsers.First(x => x.Email == user.Email).CompanyName
                                };

                                await AddToSetAsync(applicationContext.UserSet, appUser);
                                await applicationContext.SaveChangesAsync();
                            }

                            foreach (var user in accountOwnersUsers)
                            {
                                var pass = user.UserName;
                                await userManager.CreateAsync(user, pass);
                                await userManager.AddToRoleAsync(user, StringEnum.GetEnumValue<string, Description>(SecurityRoles.Tennant));

                                var appUser = new User
                                {
                                    Id = user.ApplicationUserId,
                                    Email = user.UserName,
                                    FirstName = appAccountOwnersUsers.First(x => x.Email == user.Email).FirstName,
                                    LastName = appAccountOwnersUsers.First(x => x.Email == user.Email).LastName,
                                    CompanyName = appAccountOwnersUsers.First(x => x.Email == user.Email).CompanyName
                                };

                                await AddToSetAsync(applicationContext.UserSet, appUser);
                                await applicationContext.SaveChangesAsync();
                            }

                            foreach (var user in accountFinalUsers)
                            {
                                var pass = user.UserName;
                                await userManager.CreateAsync(user, pass);
                                await userManager.AddToRoleAsync(user, StringEnum.GetEnumValue<string, Description>(SecurityRoles.User));

                                var appUser = new User
                                {
                                    Id = user.ApplicationUserId,
                                    Email = user.UserName,
                                    FirstName = appAccountFinalUsers.First(x => x.Email == user.Email).FirstName,
                                    LastName = appAccountFinalUsers.First(x => x.Email == user.Email).LastName,
                                    CompanyName = appAccountFinalUsers.First(x => x.Email == user.Email).CompanyName
                                };

                                await AddToSetAsync(applicationContext.UserSet, appUser);
                                await applicationContext.SaveChangesAsync();
                            }
                        }

                        #endregion

                        #region Report UserCreation

                        if (!applicationContext.AccountsSet.Any())
                        {
                            foreach (var account in accounts)
                            {
                                applicationContext.AccountsSet.Add(account);
                            }

                            await applicationContext.SaveChangesAsync();
                        }

                        if (!applicationContext.AccountUsersSet.Any())
                        {
                            foreach (var accountUser in accountsUsers)
                            {
                                applicationContext.AccountUsersSet.Add(accountUser);
                            }
                            await applicationContext.SaveChangesAsync();
                        }
                        #endregion
                    }
                }
            }
        }

        #endregion

        #region Application DB Manipulation
        private static async Task SetupUserTypesAsync()
        {
            ShowSimpleInfo("SetupUserTypesAsync", "Setting up user types");
            using (var context = new ApplicationDbContext(_configuration.GetConnectionString(ConnectionStrings.APPLICATION)))
            {
                await context.Database.EnsureCreatedAsync();
                if (context.AccountUserTypeSet.Count() > 0)
                {
                    ShowSimpleInfo("SetupUserTypesAsync", "AccountUserType table not empty", "Returning");
                    return;
                }
                await AddRangeToSetAsync(context.AccountUserTypeSet, JSONFiles.ACCOUNT_USER_TYPES);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SetupLanguagesAsync()
        {
            ShowSimpleInfo("SetupLanguagesAsync", "Setting up user/account languages");
            using (ApplicationDbContext context = new ApplicationDbContext(_configuration.GetConnectionString(ConnectionStrings.APPLICATION)))
            {
                await context.Database.EnsureCreatedAsync();
                if (context.LanguageSet.Any())
                {
                    ShowSimpleInfo("SetupLanguagesAsync", "Languages table not empty", "Returning");
                    return;
                }
                await AddRangeToSetAsync(context.LanguageSet, JSONFiles.LANGUAGES);
                await context.SaveChangesAsync();
            }
        }
        #endregion

        #region Logger DB Manipulation
        #endregion



        #region Helpers

        private static async Task AddRangeToSetAsync<T>(DbSet<T> set, string jsonFile) where T : class
        {
            await AddRangeToSetAsync(set, GetJsonObject<T[]>(jsonFile));
        }

        private static async Task AddToSetAsync<T>(DbSet<T> set, string jsonFile) where T : class
        {
            await AddToSetAsync(set, GetJsonObject<T>(jsonFile));
        }

        private static async Task AddRangeToSetAsync<T>(DbSet<T> set, params T[] objects) where T : class
        {
            await set.AddRangeAsync(objects);
        }

        private static async Task AddToSetAsync<T>(DbSet<T> set, params T[] objects) where T : class
        {
            await set.AddRangeAsync(objects);
        }

        private static T GetJsonObject<T>(string fileName)
        {
            return JsonConvert.DeserializeObject<T>(GetJson(fileName));
        }

        private static string GetJson(string fileName)
        {
            return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), JSONFiles.PARENT_FOLDER, fileName));
        }

        private static void ShowSimpleInfo(string methodName, params string[] texts)
        {
            Console.WriteLine($"{(string.IsNullOrEmpty(methodName) ? "" : methodName + ":")} {string.Join(" / ", texts)}");
        }

        private static void HandleGeneralException(Exception ex)
        {
            Console.WriteLine("An exception occurred while seeding the DB. Exiting...");
            StringBuilder logBuffer = new StringBuilder(ex.Message.Length + ex.StackTrace.Length + 4);
            logBuffer.AppendLine();
            logBuffer.AppendLine(ex.Message);
            logBuffer.AppendLine();
            logBuffer.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                logBuffer.AppendLine("Inner exception:");
                logBuffer.AppendLine();
                logBuffer.AppendLine(ex.InnerException.Message);
                logBuffer.AppendLine();
                logBuffer.AppendLine(ex.InnerException.StackTrace);
            }
            var log = logBuffer.ToString();
            if (_debugMode)
            {
                Console.WriteLine(log);
            }
            var logFileName = $"{DateTime.Now.ToString("yyyyMMddhhmmss")}-err-seeding.txt";
            Console.WriteLine($"Trying to log the error to {logFileName} ...");
            try
            {
                File.WriteAllText(logFileName, log);
                Console.WriteLine("Log file has been created, check deployment problems and try running the seeder again.");
            }
            catch (Exception fileEx)
            {
                Console.WriteLine("Something went terribly wrong... Can't save log :-(");
                Console.WriteLine(fileEx.Message);
                return;
            }
        }
        #endregion
    }
}
