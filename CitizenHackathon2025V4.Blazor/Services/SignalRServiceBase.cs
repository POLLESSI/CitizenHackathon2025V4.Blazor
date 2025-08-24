using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public abstract class SignalRServiceBase : IAsyncDisposable
    {
        protected HubConnection? _hubConnection;
        protected readonly string _baseHubUrl;
        protected readonly string _initialAccessToken;

        // Generic Handler Registry
        private readonly ConcurrentDictionary<Type, Delegate> _handlers = new();

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        protected SignalRServiceBase(string baseHubUrl, string accessToken)
        {
            _baseHubUrl = baseHubUrl.TrimEnd('/');
            _initialAccessToken = accessToken;
        }

        /// <summary>
        /// Initializes the generic Hub connection
        /// </summary>
        protected async Task InitializeAsync(string hubName, string? accessTokenOverride = null)
        {
            var token = accessTokenOverride ?? _initialAccessToken ?? "";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{_baseHubUrl}/{hubName}", options =>
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        options.AccessTokenProvider = () => Task.FromResult(token);
                    }
                })
                .WithAutomaticReconnect()
                .Build();

            await _hubConnection.StartAsync();
            Console.WriteLine($"[SignalRServiceBase] Connected to {_baseHubUrl}/{hubName}");
        }

        /// <summary>
        /// Registers a generic handler for a type T
        /// </summary>
        public void RegisterHandler<T>(string methodName, Action<T> handler)
        {
            //if (_hubConnection == null) throw new InvalidOperationException("HubConnection not initialized.");

            //_hubConnection.On<T>(methodName, message =>
            //{
            //    Console.WriteLine($"[SignalRServiceBase] Message received for {typeof(T).Name}");
            //    if (_handlers.TryGetValue(typeof(T), out var existing))
            //    {
            //        ((Action<T>)existing)?.Invoke(message);
            //    }
            //});

            //_handlers[typeof(T)] = handler;
            _hubConnection?.On(methodName, handler);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
    }
}










































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V4.Blazor.Client. All rights reserved.