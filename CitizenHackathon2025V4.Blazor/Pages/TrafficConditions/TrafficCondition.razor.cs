using CitizenHackathon2025V4.Blazor.Client.Common.SignalR;
using CitizenHackathon2025V4.Blazor.Client.DTOs;
using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Services;
using CitizenHackathon2025V4.Blazor.Client.Shared.Suggestion;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.TrafficConditions
{
    public partial class TrafficCondition : SignalRComponentBase<TrafficConditionModel>, IDisposable
    {
#nullable disable
        [Inject] public TrafficConditionService TrafficService { get; set; }
        [Parameter] public IEnumerable<SuggestionDTO> Suggestions { get; set; } = [];


        private int SelectedId { get; set; } = -1;
        private static string HubUrl => "/hubs/traffichub";
        private static string HubEventName => "notifyNewTraffic";
        protected override async Task<List<TrafficConditionModel>> LoadDataAsync()
        {
            var data = await TrafficService.GetLatestTrafficConditionAsync();
            return data.OrderByDescending(tc => tc.DateCondition).ToList();
        }

        protected override Task OnNewItem(TrafficConditionModel newItem)
        {
            Console.WriteLine($"🛑 New TrafficCondition received: {newItem.Description}");
            return Task.CompletedTask;
        }

        private void SelectItem(int id)
        {
            SelectedId = id;
        }

        private void HandleCrowdUpdate(CrowdInfoUIDTO data)
        {
            Console.WriteLine($"🧠 Received Crowd Info: {data.CrowdLevel} ({data.PlaceName})");
        }

        private async Task ShowOnMap()
        {
            await JS.InvokeVoidAsync("showSuggestionsOnMap", Suggestions);
        }
        protected override async Task OnInitializedAsync()
        {
            SignalRService.OnCrowdInfoUpdated += HandleCrowdUpdate;
            await SignalRService.StartAsync();
            traffic = await TrafficService.GetLatestTrafficConditionAsync();
        }
        public void Dispose()
        {
            SignalRService.OnCrowdInfoUpdated -= HandleCrowdUpdate;
        }

        private void ClickInfo(int id) => SelectedId = id;

    }
}































































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.