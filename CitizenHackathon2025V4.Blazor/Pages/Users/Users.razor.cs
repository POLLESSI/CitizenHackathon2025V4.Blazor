using CitizenHackathon2025V4.Blazor.Client.Common.SignalR;
using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;

namespace CitizenHackathon2025V4.Blazor.Client.Pages.Users
{
    public partial class User : CitizenHackathon2025V4.Blazor.Client.Common.SignalR.SignalRComponentBase<UserModel>
    {
        [Inject] public UserService UserService { get; set; }

        protected override string HubUrl => "/hubs/userhub";
        protected override string HubEventName => "notifyNewUser";
        protected override Task<List<UserModel>> LoadDataAsync()
            => UserService.GetAllActiveUsersAsync().ContinueWith(t => t.Result.ToList());
        private int SelectedId { get; set; } = -1;
        private void ClickInfo(int id) => SelectedId = id;
    }
}
















































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.