using Blazored.Toast.Services;
using CitizenHackathon2025V4.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client
{
    public partial class MainLayout : LayoutComponentBase, IAsyncDisposable
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] public IOutZenSignalRFactory SignalRFactory { get; set; }

        private OutZenSignalRService? SignalRService;

        private IJSObjectReference? _layoutModule;

        // --- Background Image Logic ---
        private string GetBackgroundImage()
        {
            var hour = DateTime.Now.Hour;
            return hour switch
            {
                < 8 => "/images/dawn.jpg",
                < 17 => "/images/day.jpg",
                < 20 => "/images/sunset.jpg",
                _ => "/images/night.jpg"
            };
        }

        private string GetTimeClass()
        {
            var hour = DateTime.Now.Hour;
            return hour switch
            {
                < 8 => "dawn",
                < 17 => "day",
                < 20 => "sunset",
                _ => "night"
            };
        }

        // --- Lifecycle Methods ---
        protected override async Task OnInitializedAsync()
        {
            //// Clean Start of SignalR
            //if (SignalRService != null)
            //{
            //    await SignalRService.InitializeOutZenAsync();
            //}
            // Dynamic construction via the factory
            SignalRService = await SignalRFactory.CreateAsync();

            // Initializing the connection
            await SignalRService.InitializeOutZenAsync();

            // SignalR Event Subscriptions
            SignalRService.OnCrowdInfoUpdated += dto =>
            {
                Console.WriteLine("📡 CrowdInfo received");
            };

            SignalRService.OnSuggestionsUpdated += suggestions =>
            {
                Console.WriteLine("📡 Suggestions received");
            };

            SignalRService.OnWeatherUpdated += forecast =>
            {
                Console.WriteLine("📡 Weather received");
            };

            SignalRService.OnTrafficUpdated += traffic =>
            {
                Console.WriteLine("📡 Traffic received");
            };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            try
            {
                // Load the layout's JS module
                _layoutModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./js/layoutCanvas.js"
                );

                // Launch canvas and parallax animations
                await _layoutModule.InvokeVoidAsync("startBackgroundCanvas");

                await JSRuntime.InvokeVoidAsync("GeometryCanvas.init");
                await JSRuntime.InvokeVoidAsync("initializeLeafletMap");
                await JSRuntime.InvokeVoidAsync("initScrollAnimations");
                await JSRuntime.InvokeVoidAsync("signalrInterop.startConnection", "/crowdHub");
                await Task.Delay(100); // Allow time for the connection to stabilize
                await JSRuntime.InvokeVoidAsync("initParallax");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ JS error in MainLayout: {ex.Message}");
            }
        }

        // --- Utility Methods ---
        private void ShowTestToast()
        {
            ToastService.ShowSuccess("It works!");
        }

        // --- Dispose JS Module & SignalR ---
        public async ValueTask DisposeAsync()
        {
            if (_layoutModule != null)
            {
                await _layoutModule.DisposeAsync();
            }

            if (SignalRService != null)
            {
                await SignalRService.DisposeAsync();
            }
        }
    }
}





































































































// Copyrigtht (c) 2025 Citizen Hackathon https://github.com/POLLESSI/Citizenhackathon2025V3.Blazor.Client. All rights reserved.