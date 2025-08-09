using CitizenHackathon2025V4.Blazor.Client.Services;
using CitizenHackathon2025V4.Blazor.Client.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.Events
{
    public partial class EventDetail : ComponentBase
    {
#nullable disable
        [Inject]
        public HttpClient? Client { get; set; }
        public EventModel? CurrentEvent { get; set; }

        [Parameter]
        public int Id { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            await GetEvent();
        }
        private async Task GetEvent()
        {
            if (Id <= 0) return;
            using (HttpResponseMessage message = await Client.GetAsync($"api/Event/{Id}"))
            {
                if (message.IsSuccessStatusCode)
                {
                    string json = await message.Content.ReadAsStringAsync();
                    CurrentEvent = JsonConvert.DeserializeObject<EventModel>(json);
                }
            }
        }
    }
}
























































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.