using Blazored.Toast.Services;
using CitizenHackathon2025V4.Blazor.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CitizenHackathon2025V4.Blazor.Client
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private IToastService ToastService { get; set; } = default!;
        [Inject] private OutZenSignalRService SignalRService { get; set; } = default!;


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

        protected override async Task OnInitializedAsync()
        {
            await SignalRService.StartAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            try
            {
                var module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./js/layoutCanvas.js"
                );

                await module.InvokeVoidAsync("startBackgroundCanvas");

                // Initial JS calls after rendering
                await JSRuntime.InvokeVoidAsync("GeometryCanvas.init");
                await JSRuntime.InvokeVoidAsync("initializeLeafletMap");
                await JSRuntime.InvokeVoidAsync("initScrollAnimations");
                await JSRuntime.InvokeVoidAsync("signalrInterop.startConnection", "/crowdHub");
                await Task.Delay(100); // Allow time for the connection
                await JSRuntime.InvokeVoidAsync("initParallax");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ JS error in MainLayout: {ex.Message}");
            }

            //if (!_initialized)
            //{
            //    _initialized = true;
            //    await OutZenService.StartAsync();
            //}
        }

        private void ShowTestToast()
        {
            ToastService.ShowSuccess("It works!");
        }
        private string GetTimeClass()
        {
            var hour = DateTime.Now.Hour;
            if (hour < 8) return "dawn";
            if (hour < 17) return "day";
            if (hour < 20) return "sunset";
            return "night";
        }
    }
}
