using System.Net.Http.Json;
using CitizenHackathon2025V4.Blazor.Client.Models;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class UserService
    {
#nullable disable
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<UserModel> GetUserByEmailAsync(string email)
        {
            var response = await _httpClient.GetAsync($"api/user/getbyemail/{email}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserModel>();
            }
            return null;
        }
        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/user/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserModel>();
            }
            return null;
        }
        public async Task<IEnumerable<UserModel>> GetAllActiveUsersAsync()
        {
            var response = await _httpClient.GetAsync("api/user/active");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<UserModel>>();
            }
            return Enumerable.Empty<UserModel>();
        }
        public async Task<UserModel> RegisterUserAsync(string email, string password, UserModel role)
        {
            var user = new UserModel
            {
                Email = email,
                PasswordHash = password, // In a real application, hash the password
                Role = role.Role,
                Status = 1 // Assuming 1 means active
            };
            var response = await _httpClient.PostAsJsonAsync("api/user/register", user);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserModel>();
            }
            throw new Exception("Failed to register user");
        }
        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = new UserModel
            {
                Email = email,
                PasswordHash = password // In a real application, hash the password
            };
            var response = await _httpClient.PostAsJsonAsync("api/user/login", user);
            return response.IsSuccessStatusCode;
        }
        public async Task DeactivateUserAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/user/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to deactivate user");
            }
        }
        public void SetRole(int id, string? role)
        {
            // This method is a placeholder for setting user roles.
            // In a real application, you would implement the logic to update the user's role.
        }
        public UserModel? UpdateUser(UserModel user)
        {
            // This method is not implemented in the original code.
            // You can implement it based on your requirements.
            throw new NotImplementedException("UpdateUser method is not implemented.");
        }
    }
}
