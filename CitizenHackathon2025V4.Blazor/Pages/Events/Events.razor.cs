using CitizenHackathon2025V4.Blazor.Client.Common.SignalR;
using CitizenHackathon2025V4.Blazor.Client.Services;
using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.Events
{
    public partial class Event
    {
#nullable disable
        [Inject] public EventService EventService { get; set; }

        protected string HubUrl => "/hubs/eventhub";
        protected string HubEventName => "notifyNewEvent";
        protected Task<List<EventModel>> LoadDataAsync() =>
             EventService.GetLatestEventAsync().ContinueWith(t => t.Result.ToList());
        private int SelectedId { get; set; } = -1;
        private void ClickInfo(int id) => SelectedId = id;
    }
}


























































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.