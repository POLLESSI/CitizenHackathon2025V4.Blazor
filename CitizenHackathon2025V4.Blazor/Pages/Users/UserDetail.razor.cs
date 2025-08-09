using CitizenHackathon2025V4.Blazor.Client.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.Users
{
    public partial class UserDetail
    {
    #nullable disable
        [Inject]
        public HttpClient? Client { get; set; }
        public UserModel? CurrentUser { get; set; }
        [Parameter]
        public int Id { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            await GetUsers();
        }

        private async Task GetUsers()
        {
            if (Id <= 0) return;

            using (HttpResponseMessage message = await Client.GetAsync($"api/User/{Id}"))
            {
                if (message.IsSuccessStatusCode)
                {
                    string json = await message.Content.ReadAsStringAsync();
                    CurrentUser = JsonConvert.DeserializeObject<UserModel>(json);
                }
            }
        }
    }
}






































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.