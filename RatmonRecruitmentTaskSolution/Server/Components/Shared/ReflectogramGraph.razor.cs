using global::Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.Data.Models;
using System;
using System.Globalization;
using System.Net.WebSockets;
using System.Reflection;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Server.Components.Shared
{
    public class ReflectogramGraphData
    {
        public int XValue { get; set; }
        public double YValue { get; set; }

        public override string ToString()
        {
            return $"X:{XValue.ToString("T")} Y:{YValue}";
        }
    }
    public partial class ReflectogramGraph
    {
        [Parameter]
        public Device Device { get; set; }

        /// <summary>
        /// display N-th newest reflectogram
        /// </summary>
        [Parameter]
        public int Number { get; set; }

        HubConnection? hubConnection;

        List<ReflectogramGraphData> graphData = new List<ReflectogramGraphData>();
        Type type;


        string FormatValue(object value)
        {
            return string.Format("{0:0.00}", ((double)value));

        }

        // quick fix to graph not updating data when there is the same amount of entries
        bool syncCheck = false;
        async Task GetData()
        {
            var devicedata = await DeviceDataService.GetLatestXDeviceDataAsync(Device.Id, Number+1);
            if (devicedata.Count <= Number) return;
            var reflectogramData = (devicedata[Number].Data as DeviceData_MOUSECOMBO).Reflectograms;

            graphData.Clear();
            int counter = 0;
            foreach (var reflectogram in reflectogramData)
            {
                var values = JsonSerializer.Deserialize<List<double>>(System.Text.Encoding.UTF8.GetString(reflectogram.Data));
                foreach(var value in values)
                {
                    graphData.Add(new ReflectogramGraphData() { XValue = counter*20, YValue = value });
                    counter++;
                }
            }
            if (syncCheck) graphData.Add(graphData[0]);
            syncCheck = !syncCheck;
        }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await GetData();

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
                await GetData();
                await InvokeAsync(StateHasChanged);
            });

            await hubConnection.StartAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }


    }
}
