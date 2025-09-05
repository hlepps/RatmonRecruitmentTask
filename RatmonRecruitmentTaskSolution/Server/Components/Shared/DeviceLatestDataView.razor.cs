using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Radzen.Blazor;
using Radzen.Blazor.Markdown;
using Server.Data.Models;
using Shared;
using System.Drawing;

namespace Server.Components.Shared
{
    public partial class DeviceLatestDataView
    {
        [Parameter]
        public Device Device { get; set; }

        List<DeviceData> devicedata = new List<DeviceData>();
        HubConnection? hubConnection;

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            devicedata = await DeviceDataService.GetLatestXDeviceDataAsync(Device.Id, 10);
            
            var url = NavigationManager.ToAbsoluteUri("/dataupdate");
            hubConnection = new HubConnectionBuilder()
            .WithUrl(url, options =>
            {
#if DEBUG                                                                           // ignoring problems with certificate during development
                var httpClientHandler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {

                        return true;

                    }
                };
                options.HttpMessageHandlerFactory = _ => httpClientHandler;
#endif
            })
            .Build();

            hubConnection.On("UpdateData", async () =>
            {
                devicedata = await DeviceDataService.GetLatestXDeviceDataAsync(Device.Id, 10);
                await InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }
    }
}
