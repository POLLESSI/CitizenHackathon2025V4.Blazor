using CitizenHackathon2025V4.Blazor.Client.Models;
using System.Net.Http.Json;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class EventService
    {
#nullable disable
        private readonly HttpClient _httpClient;

        public EventService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<EventModel?>> GetLatestEventAsync()
        {
            var response = await _httpClient.GetAsync("api/event/latest");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<EventModel?>>();
            }
            return Enumerable.Empty<EventModel?>();
        }
        public async Task<EventModel> SaveEventAsync(EventModel @event)
        {
            var response = await _httpClient.PostAsJsonAsync("api/event/save", @event);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EventModel>();
            }
            throw new Exception("Failed to save event");
        }
        public async Task<IEnumerable<EventModel>> GetUpcomingOutdoorEventsAsync()
        {
            var response = await _httpClient.GetAsync("api/event/upcoming-outdoor");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<EventModel>>();
            }
            return Enumerable.Empty<EventModel>();
        }
        public async Task<EventModel> CreateEventAsync(EventModel newEvent)
        {
            var response = await _httpClient.PostAsJsonAsync("api/event", newEvent);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EventModel>();
            }
            throw new Exception("Failed to create event");
        }
        public async Task<EventModel?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/event/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EventModel>();
            }
            return null;
        }
        public async Task<int> ArchivePastEventsAsync()
        {
            var response = await _httpClient.PostAsync("api/event/archive-expired", null);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            throw new Exception("Failed to archive past events");
        }
        public EventModel UpdateEvent(EventModel @event)
        {
            return @event; // Placeholder for actual update logic
        }
    }
}
