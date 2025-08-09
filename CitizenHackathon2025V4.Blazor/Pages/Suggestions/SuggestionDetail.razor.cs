using CitizenHackathon2025V4.Blazor.Client.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.Suggestions
{
    public partial class SuggestionDetail
    {
#nullable disable
        [Inject]
        public HttpClient? Client { get; set; }
        public SuggestionModel? CurrentSuggestion { get; set; }
        [Parameter]
        public int Id { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            await GetSuggestions();
        }

        private async Task GetSuggestions()
        {
            if (Id <= 0) return;

            using (HttpResponseMessage message = await Client.GetAsync($"api/Suggestion/{Id}"))
            {
                if (message.IsSuccessStatusCode)
                {
                    string json = await message.Content.ReadAsStringAsync();
                    CurrentSuggestion = JsonConvert.DeserializeObject<SuggestionModel>(json);
                }
            }
        }
    }
}







































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.