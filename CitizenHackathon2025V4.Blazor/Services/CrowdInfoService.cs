using System.Net.Http.Json;
using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Utils;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class CrowdInfoService
    {
#nullable disable
        private readonly HttpClient _httpClient;

        public CrowdInfoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<CrowdInfoModel?> SaveCrowdInfoAsync(CrowdInfoModel crowdInfo)
        {
            var response = await _httpClient.PostAsJsonAsync("api/crowdinfo", crowdInfo);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CrowdInfoModel>();
            }
            return null;
        }
        public async Task<IEnumerable<CrowdInfoModel>> GetAllCrowdInfoAsync()
        {
            var response = await _httpClient.GetAsync("api/crowdinfo/all");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<CrowdInfoModel>>();
            }
            return Enumerable.Empty<CrowdInfoModel>();
        }

        public async Task<List<CrowdInfoModel>> GetLatestCrowdInfoNonNullAsync()
        {
            var raw = await GetAllCrowdInfoAsync();
            return raw.ToNonNullList();
        }
        public async Task<CrowdInfoModel?> GetCrowdInfoByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/crowdinfo/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CrowdInfoModel>();
            }
            return null;
        }
        public async Task<bool> DeleteCrowdInfoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/crowdinfo/archive/{id}");
            return response.IsSuccessStatusCode;
        }
        public CrowdInfoModel UpdateCrowdInfo(CrowdInfoModel crowdInfo)
        {
            // This method is not implemented in the original code snippet.
            // You can implement it as needed, for example:
            throw new NotImplementedException("UpdateCrowdInfo method is not implemented.");
        }
    }
}
