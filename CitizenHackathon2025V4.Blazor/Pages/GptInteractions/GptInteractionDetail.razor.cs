using CitizenHackathon2025V4.Blazor.Client.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.GptInteractions
{
    public partial class GptInteractionDetail
    {
#nullable disable
        [Inject]
        public HttpClient? Client { get; set; }
        public GptInteractionModel? CurrentGptInteraction { get; set; }
        [Parameter]
        public int Id { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            await GetGptInteractions();
        }

        private async Task GetGptInteractions()
        {
            if (Id <= 0) return;

            using (HttpResponseMessage message = await Client.GetAsync($"api/gptinteractions/{Id}"))
            {
                if (message.IsSuccessStatusCode)
                {
                    string json = await message.Content.ReadAsStringAsync();
                    CurrentGptInteraction = JsonConvert.DeserializeObject<GptInteractionModel>(json);
                }
            }
        }
    }
}







































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V3.Blazor.Client. All rights reserved.