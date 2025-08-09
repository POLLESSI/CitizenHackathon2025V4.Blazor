using Blazored.Toast.Services;
using CitizenHackathon2025V4.Blazor.Client.DTOs;
using CitizenHackathon2025V4.Blazor.Client.Shared.TrafficCondition;
using CitizenHackathon2025V4.Blazor.Client.Shared.WeatherForecast;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Services
{
    public class OutZenSignalRService : IAsyncDisposable
    {
        private readonly HubConnection _connection;
        private readonly IToastService _toast;
        private readonly IJSRuntime _js;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private readonly CancellationTokenSource _cts = new();

        public bool IsConnected => _connection.State == HubConnectionState.Connected;

        public event Action<CrowdInfoUIDTO>? OnCrowdInfoUpdated;
        public event Action<WeatherForecastDTO>? OnWeatherUpdated;
        public event Action<TrafficConditionDTO>? OnTrafficUpdated;
        public event Action<HubConnectionState>? OnConnectionChanged;

        public OutZenSignalRService(NavigationManager navManager, IToastService toast, IJSRuntime js)
        {
            _toast = toast;
            _js = js;

            _connection = new HubConnectionBuilder()
                .WithUrl(navManager.ToAbsoluteUri("/hub/outzen"))
                .WithAutomaticReconnect()
                .Build();

            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            _connection.On<CrowdInfoUIDTO>("CrowdInfoUpdated", dto => OnCrowdInfoUpdated?.Invoke(dto));
            _connection.On<WeatherForecastDTO>("WeatherUpdated", dto => OnWeatherUpdated?.Invoke(dto));
            _connection.On<TrafficConditionDTO>("TrafficUpdated", dto => OnTrafficUpdated?.Invoke(dto));

            _connection.Reconnecting += async (ex) =>
            {
                await LogAndToastAsync("🔄 Reconnecting to OutZen...", LogLevel.Warning);
                OnConnectionChanged?.Invoke(HubConnectionState.Reconnecting);
            };

            _connection.Reconnected += async (connectionId) =>
            {
                await LogAndToastAsync("✅ Successfully reconnected to OutZen", LogLevel.Success);
                OnConnectionChanged?.Invoke(HubConnectionState.Connected);
            };

            _connection.Closed += async (ex) =>
            {
                await LogAndToastAsync("❌ Lost OutZen connection", LogLevel.Error);
                OnConnectionChanged?.Invoke(HubConnectionState.Disconnected);

                try
                {
                    await Task.Delay(5000, _cts.Token);
                    await TryStartAsync();
                }
                catch (OperationCanceledException) { /* Ignored */ }
            };
        }

        public async Task StartAsync()
        {
            await _connectionLock.WaitAsync();
            try
            {
                if (_connection.State == HubConnectionState.Disconnected)
                {
                    await _connection.StartAsync(_cts.Token);
                    await LogAndToastAsync("📡 Connected to OutZen (real time)", LogLevel.Success);
                    OnConnectionChanged?.Invoke(HubConnectionState.Connected);
                }
            }
            catch (Exception ex)
            {
                await LogAndToastAsync($"❌ OutZen connection error : {ex.Message}", LogLevel.Error);
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task TryStartAsync()
        {
            if (_connection.State != HubConnectionState.Disconnected) return;

            await _connectionLock.WaitAsync();
            try
            {
                await _connection.StartAsync(_cts.Token);
                await LogAndToastAsync("🔁 Reconnected to OutZen", LogLevel.Success);
                OnConnectionChanged?.Invoke(HubConnectionState.Connected);
            }
            catch
            {
                await LogAndToastAsync("⚠️ Unable to reconnect to OutZen", LogLevel.Warning);
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task StopAsync()
        {
            try
            {
                _cts.Cancel();
                await _connection.StopAsync();
                await LogAndToastAsync("⛔ Disconnected from OutZen", LogLevel.Info);
                OnConnectionChanged?.Invoke(HubConnectionState.Disconnected);
            }
            catch (Exception ex)
            {
                await LogAndToastAsync($"🚫 Disconnect error : {ex.Message}", LogLevel.Error);
            }
        }

        private async Task LogAndToastAsync(string message, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Success: _toast.ShowSuccess(message); break;
                case LogLevel.Warning: _toast.ShowWarning(message); break;
                case LogLevel.Error: _toast.ShowError(message); break;
                default: _toast.ShowInfo(message); break;
            }

            string logMethod = level switch
            {
                LogLevel.Error => "console.error",
                LogLevel.Warning => "console.warn",
                LogLevel.Success => "console.info",
                _ => "console.log"
            };

            await _js.InvokeVoidAsync(logMethod, $"[OutZen] {message}");
        }

        public async ValueTask DisposeAsync()
        {
            _cts.Cancel();
            await _connection.DisposeAsync();
            _connectionLock.Dispose();
            _cts.Dispose();
        }

        private enum LogLevel { Info, Success, Warning, Error }
    }
}
