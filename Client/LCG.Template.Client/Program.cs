using Blazored.LocalStorage;
using LCG.Template.Client.Providers;
using LCG.Template.Client.Services;
using LCG.Template.Client.Services.Contracts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace LCG.Template.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            AddServices(builder);
            AddLocalization(builder);

            var host = builder.Build();
            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
            var language = await jsInterop.InvokeAsync<string>("getLanguage");

            var culture = new CultureInfo(language);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            await host.RunAsync();
        }

        private static void AddServices(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();

            #region SINGLETONS
            #endregion

            #region TRANSIENT
            builder.Services.AddTransient<IAuthService, AuthService>();
            #endregion

            #region SCOPED
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            builder.Services.AddScoped<HttpContextAccessor>();
            #endregion
        }

        private static void AddLocalization(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
        }
    }
}
