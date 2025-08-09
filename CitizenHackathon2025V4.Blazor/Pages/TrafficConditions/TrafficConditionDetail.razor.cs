using CitizenHackathon2025V4.Blazor.Client.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.TrafficConditions
{
    public partial class TrafficConditionDetail
    {
#nullable disable
        [Inject]
        public HttpClient? Client { get; set; }
        public TrafficConditionModel? CurrentTrafficCondition { get; set; }
        [Parameter]
        public int Id { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            await GetTrafficConditions();
        }

        private async Task GetTrafficConditions()
        {
            if (Id <= 0) return;

            using (HttpResponseMessage message = await Client.GetAsync($"api/TrafficCondition/{Id}"))
            {
                if (message.IsSuccessStatusCode)
                {
                    string json = await message.Content.ReadAsStringAsync();
                    CurrentTrafficCondition = JsonConvert.DeserializeObject<TrafficConditionModel>(json);
                }
            }
        }
    }
}

















































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.