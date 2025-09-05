using global::Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.Data.Models;
using Server.Migrations;
using System.Globalization;
using System.Reflection;
using static System.Net.WebRequestMethods;

namespace Server.Components.Shared
{
    public class GraphData
    {
        public DateTime XValue { get; set; }
        public double YValue { get; set; }

        public override string ToString()
        {
            return $"X:{XValue.ToString("T")} Y:{YValue}";
        }
    }

    public enum ValueFormat
    {
        Normal, Temperature, Percent
    }

    public partial class ParameterGraph
    {
        [Parameter]
        public Device Device { get; set; }

        [Parameter]
        public string DataParameterName { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public ValueFormat ValueFormat { get; set; }

        HubConnection? hubConnection;

        List<GraphData> graphData = new List<GraphData>();
        Type type;

        string FormatDateTime(object value)
        {
            return ((DateTime)value).ToLocalTime().ToString("T");
        }

        string FormatValue(object value)
        {
            switch(ValueFormat)
            {
                case ValueFormat.Normal:
                    return string.Format("{0:0.00}", ((double)value));
                    break;
                case ValueFormat.Temperature:
                    return string.Format("{0:0.00°C}", ((double)value));
                    break;
                case ValueFormat.Percent:
                    return string.Format("{0:0.00%}", ((double)value));
                    break;
            }
            return string.Format("{0:0.00}", ((double)value));

        }

        async Task GetData()
        {
            switch (Device.Type)
            {
                case DeviceType.MOUSE2:
                    type = typeof(DeviceData_MOUSE2);
                    break;
                case DeviceType.MOUSE2B:
                    type = typeof(DeviceData_MOUSE2B);
                    break;
                case DeviceType.MOUSECOMBO:
                    type = typeof(DeviceData_MOUSECOMBO);
                    break;
                case DeviceType.MAS2:
                    type = typeof(DeviceData_MAS2);
                    break;
                default:
                    type = typeof(DeviceData_MOUSE2);
                    break;

            }
            var devicedata = await DeviceDataService.GetLatestXDeviceDataAsync(Device.Id, 10);

            foreach (var single in devicedata)
            {
                graphData.Add(new GraphData() { XValue = single.Timestamp, YValue = (double)type.GetProperty(DataParameterName).GetValue(single.Data) });
            }

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
