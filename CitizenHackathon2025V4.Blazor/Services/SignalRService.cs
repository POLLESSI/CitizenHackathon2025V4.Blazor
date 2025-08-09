using CitizenHackathon2025V4.Blazor.Client.DTOs;
using CitizenHackathon2025V4.Blazor.Client.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class SignalRService : ISignalRService
    {
        private HubConnection _connection;
        public event Func<object, Task> OnNotify;
        public event Func<CrowdInfoUIDTO, Task> OnCrowdInfoUpdated;
        public event Func<TrafficConditionModel, Task> OnTrafficUpdated;
        public event Func<WeatherForecastModel, Task> OnWeatherForecastUpdated;

        public async Task StartAsync(string hubUrl, string hubEventName)
        {
            if (_connection != null && _connection.State == HubConnectionState.Connected)
                return;

            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _connection.On<CrowdInfoUIDTO>("crowdInfoUpdate", async (data) =>
            {
                if (OnCrowdInfoUpdated != null)
                    await OnCrowdInfoUpdated(data);
            });

            await _connection.StartAsync();
        }

        public async Task StopAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}
