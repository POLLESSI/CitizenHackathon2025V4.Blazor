using CitizenHackathon2025V4.Blazor.Client.Common.SignalR;
using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.WeatherForecasts
{
    public partial class WeatherForecast : CitizenHackathon2025V4.Blazor.Client.Common.SignalR.SignalRComponentBase<WeatherForecastModel>
    {
#nullable disable
        [Inject] public WeatherForecastService WeatherService { get; set; }

        protected override string HubUrl => "/hubs/weatherhub";
        protected override string HubEventName => "notifyNewForecast";
        protected override Task<List<WeatherForecastModel>> LoadDataAsync()
            => WeatherService.GetLatestWeatherForecastAsync().ContinueWith(t => t.Result.ToList());


        private int SelectedId { get; set; } = -1;
        private void ClickInfo(int id) => SelectedId = id;
    }
}







































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.