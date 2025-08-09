using CitizenHackathon2025V4.Blazor.Client.Common.SignalR;
using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Services;
using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.CrowdInfos
{
    public partial class CrowdInfo : CitizenHackathon2025V4.Blazor.Client.Common.SignalR.SignalRComponentBase<CrowdInfoModel>
    {
#nullable disable
        [Inject] public CrowdInfoService CrowdInfoService { get; set; }

        protected override string HubUrl => "/hubs/crowdhub";
        protected override string HubEventName => "notifynewCrowd";
        protected override Task<List<CrowdInfoModel>> LoadDataAsync()
            => CrowdInfoService.GetAllCrowdInfoAsync().ContinueWith(t => t.Result.ToList());
        private int SelectedId { get; set; } = -1;
        private void ClickInfo(int id) => SelectedId = id;
    }
}

