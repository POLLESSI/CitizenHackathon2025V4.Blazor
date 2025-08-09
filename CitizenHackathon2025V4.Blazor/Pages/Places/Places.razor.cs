using CitizenHackathon2025V4.Blazor.Client.Common.SignalR;
using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.Places
{
    public partial class Place : CitizenHackathon2025V4.Blazor.Client.Common.SignalR.SignalRComponentBase<PlaceModel>
    {
#nullable disable
        [Inject] public PlaceService PlaceService { get; set; }

        protected override string HubUrl => "/hubs/placehub";
        protected override string HubEventName => "notifyNewPlace";
        protected override Task<List<PlaceModel>> LoadDataAsync()
            => PlaceService.GetLatestPlaceAsync().ContinueWith(t => t.Result.ToList());
        private int SelectedId { get; set; } = -1;
        private void ClickInfo(int id) => SelectedId = id;
    }
}































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.