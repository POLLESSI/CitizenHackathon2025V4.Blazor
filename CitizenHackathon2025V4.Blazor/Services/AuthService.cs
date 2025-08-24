using CitizenHackathon2025V4.Blazor.Client.Pages.Auths;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        private const string TokenKey = "jwt_token";

        public AuthService(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var payload = new { email, password };
            var response = await _httpClient.PostAsJsonAsync("auth/login", payload);

            if (!response.IsSuccessStatusCode) return false;

            var token = await response.Content.ReadAsStringAsync();
            await JSInterop.SetLocalStorage(TokenKey, token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return true;
        }

        public async Task LogoutAsync()
        {
            await JSInterop.RemoveLocalStorage(TokenKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<JwtPayload?> GetCurrentUserAsync()
        {
            var token = await JSInterop.GetLocalStorage(TokenKey);
            if (string.IsNullOrEmpty(token)) return null;

            try
            {
                var payload = JwtParser.DecodePayload(token);
                // configure HttpClient header if not set
                if (_httpClient.DefaultRequestHeaders.Authorization == null)
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return payload;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> GetTokenAsync() => await JSInterop.GetLocalStorage(TokenKey);

    }
}








































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.