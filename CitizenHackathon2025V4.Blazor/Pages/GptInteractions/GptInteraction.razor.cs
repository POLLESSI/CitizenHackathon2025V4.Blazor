using Microsoft.AspNetCore.Components;
using CitizenHackathon2025V4.Blazor.Client.Services;
using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Common.SignalR;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Linq;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.GptInteractions
{
    public partial class GptInteraction : SignalRComponentBase<GptInteractionModel>
    {
        private List<TrafficConditionModel> TrafficConditionsList = new();
        [Inject] public TrafficConditionService TrafficService { get; set; }
        [Inject] public GptInteractionService? GptInteractionService { get; set; } = default!;

        private bool IsLoadingMore = false;

        protected override string HubUrl => "/hubs/gpthub";
        protected override string HubEventName => "notifyNewGptInteraction";

        private ElementReference ScrollContainerRef;
        private int SelectedId { get; set; } = -1;

        protected override async Task<List<GptInteractionModel>> LoadDataAsync()
        {
            var interactions = await GptInteractionService!.GetAllInteractions();
            return interactions.ToList(); // ✅ conversion explicite
        }

        private GptInteractionModel? SelectedItem =>
            VisibleItems.FirstOrDefault(x => x.Id == SelectedId);

        private string GetRowClass(GptInteractionModel item)
            => item.Id == SelectedId ? "table-primary" : string.Empty;

        private void SelectItem(int id)
            => SelectedId = id;

        protected override async Task OnInitializedAsync()
        {
            var rawTraffic = await TrafficService.GetLatestTrafficConditionAsync();
            TrafficConditionsList = rawTraffic
                .Where(t => t is not null)
                .Select(t => t!)
                .ToList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadInitialItemsAsync();
                await JS.InvokeVoidAsync("trafficInterop.updateTrafficMarkers", TrafficConditionsList);
            }
        }

        private async Task HandleScroll()
        {
            if (IsLoading || IsLoadingMore) return;

            var scrollTop = await JS.InvokeAsync<int>("scrollInterop.getScrollTop", ScrollContainerRef);
            var scrollHeight = await JS.InvokeAsync<int>("scrollInterop.getScrollHeight", ScrollContainerRef);
            var clientHeight = await JS.InvokeAsync<int>("scrollInterop.getClientHeight", ScrollContainerRef);

            var scrollThreshold = 40;

            if (scrollTop + clientHeight + scrollThreshold >= scrollHeight)
            {
                IsLoadingMore = true;
                StateHasChanged();

                try
                {
                    await LoadMoreItemsAsync();
                }
                finally
                {
                    IsLoadingMore = false;
                    StateHasChanged();
                }
            }
        }
    }
}


































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.