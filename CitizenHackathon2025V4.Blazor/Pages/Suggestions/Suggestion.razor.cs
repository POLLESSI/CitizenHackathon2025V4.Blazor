using CitizenHackathon2025V4.Blazor.Client.Shared.Suggestion;
using CitizenHackathon2025V4.Blazor.Client.Common.SignalR;
using CitizenHackathon2025V4.Blazor.Client.DTOs;
using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.Suggestions
{
    public partial class Suggestion : SignalRComponentBase<SuggestionModel>
    {
#nullable disable
        //[Inject] public SuggestionService SuggestionService { get; set; }
        private int SelectedId { get; set; } = -1;

        private static string HubUrl => "/hubs/suggestionhub";
        private static string HubEventName => "notifyNewSuggestion";
        protected override async Task<List<SuggestionModel>> LoadDataAsync()
        {
            var data = await SuggestionService.GetLatestSuggestionAsync();
            return data.OrderByDescending(s => s.Date).ToList();
        }
        protected override Task OnNewItem(SuggestionModel newItem)
        {
            Console.WriteLine($"💡 New Suggestion received: {newItem.OriginalPlace} -> {newItem.SuggestedAlternative}");
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
            await JS.InvokeVoidAsync("showSuggestionsOnMap", SuggestionsList);
        }
        protected override async Task OnInitializedAsync()
        {
            SignalRService.OnCrowdInfoUpdated += HandleCrowdUpdate;
            await SignalRService.StartAsync();
        }
        public void Dispose()
        {
            SignalRService.OnCrowdInfoUpdated -= HandleCrowdUpdate;
        }
        private void ClickInfo(int id) => SelectedId = id;
    }
}


























































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.