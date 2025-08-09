using System.Net.Http.Json;
using CitizenHackathon2025V4.Blazor.Client.Models;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class OutZenService
    {
    #nullable disable
        private readonly HttpClient _httpClient;

        public OutZenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
