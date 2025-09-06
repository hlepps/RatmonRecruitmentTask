using Microsoft.AspNetCore.Components;
using Server.Data.Models;

namespace Server.Components.Pages
{
    public partial class DeviceConfig
    {
        [Parameter]
        public string DeviceId { get; set; }

        DateTime StartDate { get; set; } = DateTime.Now.AddHours(-1);
        DateTime EndDate { get; set; } = DateTime.Now;

        Device currentDevice;
        List<DeviceData> currentDeviceData = new List<DeviceData>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await UpdateData();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            await UpdateData();
        }

        async Task StartDateTimeChanged(DateTime newDateTime)
        {
            StartDate = newDateTime;
            currentDeviceData.Clear();
            await InvokeAsync(StateHasChanged);
            await UpdateData();
        }
        async Task EndDateTimeChanged(DateTime newDateTime)
        {
            EndDate = newDateTime;
            currentDeviceData.Clear();
            await InvokeAsync(StateHasChanged);
            await UpdateData();
        }
        // quick fix to graph not updating data when there is the same amount of entries
        bool syncCheck = false;
        async Task UpdateData()
        {
            currentDevice = await DeviceService.GetDeviceByIdAsync(DeviceId);
            currentDeviceData = await deviceDataService.GetLatestDeviceDataBetweenDatesAsync(DeviceId, StartDate.ToUniversalTime(), EndDate.ToUniversalTime());
            if (currentDeviceData.Count != 0 && syncCheck)
            {
                //currentDeviceData.Add(currentDeviceData[0]);
            }
            syncCheck = !syncCheck;
            await InvokeAsync(StateHasChanged);
        }
    }
}
