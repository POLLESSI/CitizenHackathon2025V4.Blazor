namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class OutZenSignalRFactory : IOutZenSignalRFactory
    {
    #nullable disable
        private readonly UserService _userService;
        private readonly EventService _eventService;

        public OutZenSignalRFactory(UserService userService, EventService eventService)
        {
            _userService = userService;
            _eventService = eventService;
        }
        public async Task<OutZenSignalRService> CreateAsync()
        {
            var eventId = _eventService.GetCurrentEvent() ?? "default-event";

            return new OutZenSignalRService(
                "https://localhost:7254",
                () => _userService.GetAccessTokenAsync(), // 🔹 provider dynamique
                eventId,
                "https://localhost:7254/hubs/outzen"
            );
        }
    }
}






































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.