using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json.Serialization;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.Places
{
    public partial class PlaceView
    {
    #nullable disable
        [Inject]
        public HttpClient Client { get; set; }  // Injection HttpClient
        [Inject] public PlaceService PlaceService { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        public List<PlaceModel> Places { get; set; } = new();
        public int SelectedId { get; set; }
        public HubConnection hubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Places = new List<PlaceModel>();

            await GetPlaces();

            hubConnection = new HubConnectionBuilder()
                .WithUrl(new Uri("https://localhost:7254/hubs/placeHub"))
                .Build();

            await hubConnection.StartAsync();
        }
        private void ClickInfo(int id) => SelectedId = id;

        private async Task GetPlaces()
        {
            using (HttpResponseMessage message = await Client.GetAsync("place"))
            {
                if (message.IsSuccessStatusCode)
                {
                    string json = await message.Content.ReadAsStringAsync();
                    Places = JsonConvert.DeserializeObject<List<PlaceModel>>(json);
                    // Process places as needed
                }
                else
                {
                    // Handle error response
                }
            }
        }
    }
}































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.