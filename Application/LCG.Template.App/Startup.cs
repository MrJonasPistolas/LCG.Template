using LCG.Template.App.Areas.Identity;
using LCG.Template.App.Data;
using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Tools.Logger;
using LCG.Template.Common.Tools.Utils;
using LCG.Template.Data.Application;
using LCG.Template.Data.Identity;
using LCG.Template.Data.Logging;
using LCG.Template.ServiceContracts;
using LCG.Template.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;

namespace LCG.Template.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddAllDbContext(services);
            AddAndConfigureIdentity(services);
            AddServices(services);
            AddLocalization(services);

            services.AddSession();
            services.AddLogging();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
            services.AddRazorPages();
            services.AddServerSideBlazor();
            //services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            services.AddDatabaseDeveloperPageExceptionFilter();
            //services.AddSingleton<WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            var connection = Configuration.GetConnectionString("Logger");
            loggerFactory.AddProvider(new LoggerDatabaseProvider(connection, httpContextAccessor));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Begin I18N configuration
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("pt"),
                new CultureInfo("en") 
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("pt"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            app.UseRequestLocalization(options);
            // End I18N configuration

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseMvcWithDefaultRoute();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        /// <summary>
        /// In this method you should add all the DbContext the app is going to use
        /// </summary>
        /// <param name="services">Service Collection</param>
        private void AddAllDbContext(IServiceCollection services)
        {
            services.AddDbContext<SecurityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Security")));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Application")));
            services.AddDbContext<LoggerDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Logger")));
        }

        /// <summary>
        /// In this method you should add and configure your identity
        /// </summary>
        /// <param name="services">Service Collection</param>
        private void AddAndConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<SecurityDbContext>()
               .AddDefaultTokenProviders();

            services.Configure<PasswordHasherOptions>(options =>
            {
                options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
            });
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });
        }

        private void AddServices(IServiceCollection services)
        {
            #region SINGLETONS
            
            #endregion

            #region TRANSIENT

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<ISessionService, SessionService>();
            services.AddTransient<IUrlUtils, URLUtils>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<WeatherForecastService>();
            #endregion

            #region SCOPED
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            #endregion

            services.AddHttpContextAccessor();
        }

        private static void AddLocalization(IServiceCollection services)
        {
            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
        }
    }
}
