using CitizenHackathon2025V4.Blazor.Client.Models;
using CitizenHackathon2025V4.Blazor.Client.Utils;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class TrafficConditionService
    {
#nullable disable
        private readonly HttpClient _httpClient;

        public TrafficConditionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<TrafficConditionModel?>> GetLatestTrafficConditionAsync()
        {

            //var response = await _httpClient.GetAsync("api/trafficcondition/latest");
            //if (response.IsSuccessStatusCode)
            //{
            //    var data = await response.Content.ReadFromJsonAsync<IEnumerable<TrafficConditionModel>>();
            //    return data?.ToList() ?? new List<TrafficConditionModel>();
            //}

            //return new List<TrafficConditionModel>();
            try
            {
                var raw = await GetLatestTrafficConditionAsync();
                return raw.ToNonNullList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Erreur GetLatestTrafficConditionAsync : " + ex.Message);
                return Enumerable.Empty<TrafficConditionModel>();
            }
        }
        public async Task<TrafficConditionModel> SaveTrafficConditionAsync(TrafficConditionModel @trafficCondition)
        {
            var response = await _httpClient.PostAsJsonAsync("api/trafficcondition", @trafficCondition);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TrafficConditionModel>();
            }
            throw new Exception("Failed to save traffic condition");
        }
        public TrafficConditionModel? UpdateTrafficCondition(TrafficConditionModel @trafficCondition)
        {
            // This method is not implemented in the original code.
            // You can implement it based on your requirements.
            throw new NotImplementedException("UpdateTrafficCondition method is not implemented.");
        }
    }
}
