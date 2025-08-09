using CitizenHackathon2025V4.Blazor.Client.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.WeatherForecasts
{
    public partial class WeatherForecastDetail
    {
#nullable disable
        [Inject]
        public HttpClient? Client { get; set; }
        public WeatherForecastModel? CurrentWeatherForecast { get; set; }
        [Parameter]
        public int Id { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            await GetWeatherForecasts();
        }

        private async Task GetWeatherForecasts()
        {
            if (Id <= 0) return;

            using (HttpResponseMessage message = await Client.GetAsync($"api/WeatherForecast/{Id}"))
            {
                if (message.IsSuccessStatusCode)
                {
                    string json = await message.Content.ReadAsStringAsync();
                    CurrentWeatherForecast = JsonConvert.DeserializeObject<WeatherForecastModel>(json);
                }
            }
        }
    }
}






















































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.