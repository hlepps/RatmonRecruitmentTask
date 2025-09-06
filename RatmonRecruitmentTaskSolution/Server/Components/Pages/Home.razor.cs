using Server.Data.Models;

namespace Server.Components.Pages
{
    public partial class Home
    {
        List<Device> devices = new List<Device>();
        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();

            devices = await DeviceService.GetAllRegisteredDevicesAsync();
        }
    }
}
