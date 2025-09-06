using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Server.Data.Models;
using Server.Services;

namespace Server.Components.Layout
{
    public partial class NavMenu : ComponentBase
    {
        private string? currentUrl;

        List<Device> devices = new List<Device>();

        protected override void OnInitialized()
        {
            currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();

            devices = await DeviceService.GetAllRegisteredDevicesAsync();
            //await InvokeAsync(StateHasChanged);
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
            StateHasChanged();
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }
    }
}
