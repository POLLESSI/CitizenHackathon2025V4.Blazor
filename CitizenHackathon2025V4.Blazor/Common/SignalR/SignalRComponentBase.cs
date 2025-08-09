using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using CitizenHackathon2025V4.Blazor.Client.Services;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenHackathon2025V4.Blazor.Client.Common.SignalR
{
    public abstract class SignalRComponentBase<TModel> : ComponentBase, IAsyncDisposable
    {
#nullable disable

        [Inject] protected ISignalRService SignalRService { get; set; }
        [Inject] protected IJSRuntime JS { get; set; }

        protected HubConnection hubConnection;
        public List<TModel> Items { get; private set; } = new();

        protected bool IsLoading = true;
        public List<TModel> VisibleItems { get; private set; } = new();

        protected int BatchSize { get; set; } = 10;
        protected int CurrentIndex { get; private set; } = 0;
        protected bool IsEndOfList => CurrentIndex >= Items.Count;
        protected string ErrorMessage { get; private set; }

        /// <summary>
        /// Will be triggered on each new item received via SignalR.
        /// </summary>
        protected abstract Task<List<TModel>> LoadDataAsync();
        protected virtual Task OnNewItem(TModel newItem) => Task.CompletedTask;

        protected virtual string HubUrl => "/hubs/notifyhub";
        protected virtual string HubEventName => "notify";


        protected override async Task OnInitializedAsync()
        {
            SignalRService.OnNotify += async (data) =>
            {
                if (data is TModel typedData)
                {
                    Items.Insert(0, typedData);
                    await OnNewItem(typedData);
                    StateHasChanged();
                }
            };

            await SignalRService.StartAsync(HubUrl, HubEventName);
            Items = await LoadDataAsync();
            IsLoading = false;
        }

        /// <summary>
        /// Loads the next portion of items.
        /// </summary>
        protected void LoadNextPage()
        {
            if (IsEndOfList) return;

            var nextBatch = Items.Skip(CurrentIndex).Take(BatchSize).ToList();
            VisibleItems.AddRange(nextBatch);
            CurrentIndex += BatchSize;
            StateHasChanged();
        }

        private async Task InitializeHubConnection()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(HubUrl, options =>
                {
                    options.SkipNegotiation = true;
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
                })
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On<TModel>(HubEventName, async model =>
            {
                Items.Insert(0, model);
                VisibleItems.Insert(0, model);
                CurrentIndex++;

                await InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection != null)
            {
                await hubConnection.DisposeAsync();
            }
        }
        protected virtual Task OnNewItemReceived(TModel item)
        {
            return Task.CompletedTask;
        }
        /// <summary>
        /// Initial loading on initial display (1st page).
        /// </summary>
        protected virtual async Task LoadInitialItemsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                Items = await LoadDataAsync() ?? new();
                VisibleItems.Clear();
                CurrentIndex = 0;

                LoadNextPage(); // ➕ loads the first batch
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur de chargement initial : {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Loading the next page (scroll down).
        /// </summary>
        protected virtual async Task LoadMoreItemsAsync()
        {
            if (IsEndOfList) return;

            try
            {
                IsLoading = true;
                LoadNextPage(); // ➕ use existing method
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur de pagination : {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
