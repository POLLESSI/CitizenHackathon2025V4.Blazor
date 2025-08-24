using Blazored.Toast.Services;
using CitizenHackathon2025V4.Blazor.Client.DTOs;
using CitizenHackathon2025V4.Blazor.Client.Shared.CrowdInfo;
using CitizenHackathon2025V4.Blazor.Client.Shared.Suggestion;
using CitizenHackathon2025V4.Blazor.Client.Shared.TrafficCondition;
using CitizenHackathon2025V4.Blazor.Client.Shared.WeatherForecast;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class OutZenSignalRService : SignalRServiceBase
    {
        private readonly string _eventId;
        private readonly string _hubUrl;
        private readonly Func<Task<string?>> _accessTokenProvider;

        public event Action<CrowdInfoDTO>? OnCrowdInfoUpdated;
        public event Action<List<SuggestionDTO>>? OnSuggestionsUpdated;
        public event Action<WeatherForecastDTO>? OnWeatherUpdated;
        public event Action<TrafficConditionDTO>? OnTrafficUpdated;

        public OutZenSignalRService(
            string baseHubUrl,
            Func<Task<string?>> accessTokenProvider,
            string eventId,
            string hubUrl
        ) : base(baseHubUrl, "") // ⚠️ we pass empty to the SignalRServiceBase
        {
            _eventId = eventId;
            _hubUrl = hubUrl;
            _accessTokenProvider = accessTokenProvider;
        }

        public async Task InitializeOutZenAsync()
        {
            // ✅ A dynamic token provider is provided at login time
            var token = await _accessTokenProvider.Invoke() ?? "";

            await InitializeAsync("outzenhub", token);

            RegisterHandler<CrowdInfoDTO>("CrowdInfoUpdated", dto =>
            {
                Console.WriteLine("[OutZenSignalRService] CrowdInfo event handled");
                OnCrowdInfoUpdated?.Invoke(dto);
            });

            RegisterHandler<List<SuggestionDTO>>("SuggestionsUpdated", suggestions =>
            {
                Console.WriteLine("[OutZenSignalRService] Suggestions handled");
                OnSuggestionsUpdated?.Invoke(suggestions);
            });

            RegisterHandler<WeatherForecastDTO>("WeatherUpdated", forecast =>
            {
                Console.WriteLine("[OutZenSignalRService] Weather handled");
                OnWeatherUpdated?.Invoke(forecast);
            });

            RegisterHandler<TrafficConditionDTO>("TrafficUpdated", traffic =>
            {
                Console.WriteLine("[OutZenSignalRService] Traffic handled");
                OnTrafficUpdated?.Invoke(traffic);
            });

            await JoinEventGroupAsync(_eventId);
        }

        public async Task JoinEventGroupAsync(string eventId)
        {
            if (_hubConnection != null && IsConnected)
            {
                await _hubConnection.InvokeAsync("JoinEventGroup", eventId);
                Console.WriteLine($"[OutZenSignalRService] Joined group event-{eventId}");
            }
        }

        public async Task LeaveEventGroupAsync(string eventId)
        {
            if (_hubConnection != null && IsConnected)
            {
                await _hubConnection.InvokeAsync("LeaveEventGroup", eventId);
                Console.WriteLine($"[OutZenSignalRService] Left group event-{eventId}");
            }
        }
    }
}
























































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.