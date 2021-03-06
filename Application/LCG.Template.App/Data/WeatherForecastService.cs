using LCG.Template.Common.Enums.Auth;
using LCG.Template.Common.Models.Account;
using LCG.Template.ServiceContracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LCG.Template.App.Data
{
    public class WeatherForecastService
    {
        private readonly ISessionService _sessionService;
        public WeatherForecastService (ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray());
        }

        public async Task<LoginStatus> Cenas(LoginModel loginModel)
        {
            return await _sessionService.LoginAndSetSessionInfoAsync(loginModel);
        }
    }
}
