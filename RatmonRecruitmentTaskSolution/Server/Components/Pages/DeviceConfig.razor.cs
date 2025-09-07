using DeviceBase;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Server.Data.Models;
using Shared;
using System.Text.Json;

namespace Server.Components.Pages
{
    public partial class DeviceConfig
    {
        [Parameter]
        public string DeviceId { get; set; }

        Device currentDevice;
        Config deviceConfig;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            currentDevice = await DeviceService.GetDeviceByIdAsync(DeviceId);
            var jsonConfig = await RabbitMQDeviceConfigService.CallAsync(RpcConfigMessageType.GET_CONFIG, "", currentDevice.Id);
            
            switch(currentDevice.Type)
            {
                case DeviceType.MOUSE2:
                    deviceConfig = JsonSerializer.Deserialize<Config_MOUSE2>(jsonConfig)!;
                    break;
                case DeviceType.MOUSE2B:
                    deviceConfig = JsonSerializer.Deserialize<Config_MOUSE2B>(jsonConfig)!;
                    break;
                case DeviceType.MOUSECOMBO:
                    deviceConfig = JsonSerializer.Deserialize<Config_MOUSECOMBO>(jsonConfig)!;
                    Console.WriteLine((deviceConfig as Config_MOUSECOMBO).AlarmThreshold);
                    break;
                case DeviceType.MAS2:
                    deviceConfig = JsonSerializer.Deserialize<Config_MAS2>(jsonConfig)!;
                    break;
            }
        }

        private async Task RemoveDialog()
        {
            bool? result = await DialogService.Confirm(
                    $"Are you sure to delete {currentDevice.Name} and all of its data? If the device is still sending data, it will be registered again.",
                    "Confirm",
                    new ConfirmOptions()
                    {
                        OkButtonText = "Yes",
                        CancelButtonText = "No",
                        CloseDialogOnOverlayClick = true,
                        CloseDialogOnEsc = true,
                    }
                );

            if (result == true)
            {
                await DeviceService.RemoveDeviceFromDatabase(currentDevice.Id);
                NavigationManager.NavigateTo("/", true);
            }
        }
        async Task SaveConfig()
        {
            string serialized = "";
            switch (currentDevice.Type)
            {
                case DeviceType.MOUSE2:
                    serialized = JsonSerializer.Serialize(deviceConfig as Config_MOUSE2)!;
                    break;
                case DeviceType.MOUSE2B:
                    serialized = JsonSerializer.Serialize(deviceConfig as Config_MOUSE2B)!;
                    break;
                case DeviceType.MOUSECOMBO:
                    serialized = JsonSerializer.Serialize(deviceConfig as Config_MOUSECOMBO)!;
                    break;
                case DeviceType.MAS2:
                    serialized = JsonSerializer.Serialize(deviceConfig as Config_MAS2)!;
                    break;
            }


            var response = await RabbitMQDeviceConfigService.CallAsync(RpcConfigMessageType.UPDATE_CONFIG, serialized, currentDevice.Id);

            if(response == serialized)
            {
                NotificationService.Notify(new Radzen.NotificationMessage { Severity=Radzen.NotificationSeverity.Success, Summary="Success", Detail="Config successfully saved in device", Duration=4000});
                if(deviceConfig.Name != currentDevice.Name)
                {
                    await DeviceService.UpdateDeviceNameAsync(currentDevice.Id, deviceConfig.Name);
                    InvokeAsync(StateHasChanged);
                }
            }
            else
            {
                NotificationService.Notify(new Radzen.NotificationMessage { Severity = Radzen.NotificationSeverity.Error, Summary = "Error", Detail = $"Could't save config. Message received from device: {response}", Duration = 4000 });
            }
        }

    }
}
