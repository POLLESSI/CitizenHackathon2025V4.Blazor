using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Pages.GptInteractions;
using System.Net.Http.Json;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class GptInteractionService
    {
#nullable disable
        private readonly HttpClient _httpClient;

        public GptInteractionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<GptInteractionModel>> GetAllInteractions()
        {
            var response = await _httpClient.GetAsync($"api/gpt/all");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<GptInteractionModel>>();
            }
            return Enumerable.Empty<GptInteractionModel>();
        }
        public async Task<IEnumerable<GptInteractionModel>> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"api/gpt/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<GptInteractionModel>>();
            }
            return Enumerable.Empty<GptInteractionModel>();
        }
        //public async Task<IEnumerable<GptInteractionModel>> GetSuggestionsByForecastIdAsync(int id)
        //{
        //    var response = await _httpClient.GetAsync($"api/gpt/forecast/{id}");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return await response.Content.ReadFromJsonAsync<IEnumerable<GptInteractionModel>>();
        //    }
        //    return Enumerable.Empty<GptInteractionModel>();
        //}
        //public async Task<IEnumerable<GptInteractionModel>> GetSuggestionsByTrafficIdAsync(int id)
        //{
        //    var response = await _httpClient.GetAsync($"api/gpt/traffic/{id}");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return await response.Content.ReadFromJsonAsync<IEnumerable<GptInteractionModel>>();
        //    }
        //    return Enumerable.Empty<GptInteractionModel>();
        //}
        public async Task AskGpt(GptInteractionModel prompt)
        {
            var response = await _httpClient.PostAsJsonAsync("api/gpt/ask-gpt", prompt);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to save suggestion");
            }
        }
        public async Task Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/gpt/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to delete suggestion");
            }
        }
        public async Task ReplayInteraction(int id)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/gpt/replay/{id}", new { });
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to replay interaction");
            }
        }
    }
}
