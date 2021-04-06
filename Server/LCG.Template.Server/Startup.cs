using LCG.Template.Common.Entities.Identity;
using LCG.Template.Common.Tools.Logger;
using LCG.Template.Common.Tools.Utils;
using LCG.Template.Data.Application;
using LCG.Template.Data.Identity;
using LCG.Template.Data.Logging;
using LCG.Template.ServiceContracts;
using LCG.Template.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LCG.Template.Server
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
            AddAndConfigureSwagger(services);

            services.AddSession();
            services.AddLogging();

            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            var connection = Configuration.GetConnectionString("Logger");
            loggerFactory.AddProvider(new LoggerDatabaseProvider(connection, httpContextAccessor));
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LCG Template API");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }

        /// <summary>
        /// In this method you should add all the DbContext the app is going to use
        /// </summary>
        /// <param name="services"></param>
        private void AddAllDbContext(IServiceCollection services)
        {
            services.AddDbContext<SecurityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Security")));
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Application")));
            services.AddDbContext<LoggerDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Logger")));

            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        /// <summary>
        /// In this method you should add and configure your identity
        /// </summary>
        /// <param name="services"></param>
        private void AddAndConfigureIdentity(IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<SecurityDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["JwtIssuer"],
                            ValidAudience = Configuration["JwtAudience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityKey"]))
                        };
                    });

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        /// <summary>
        /// In this method you should add and configure your Swagger service
        /// </summary>
        /// <param name="services"></param>
        private void AddAndConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "LCG Template API",
                    Description = "List of method allowed to be used on LCG Template API",
                    TermsOfService = new Uri("https://lcg.consulting/privacidade")
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";
                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });
        }

        /// <summary>
        /// In this method you should add and configure your DI Services
        /// </summary>
        /// <param name="services"></param>
        private void AddServices(IServiceCollection services)
        {
            #region SINGLETONS
            #endregion

            #region TRANSIENT
            services.AddTransient<ICypherService, CypherService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IUrlUtils, URLUtils>();
            services.AddTransient<IEmailService, EmailService>();
            #endregion

            #region SCOPED
            #endregion

            services.AddHttpContextAccessor();
        }
    }
}
