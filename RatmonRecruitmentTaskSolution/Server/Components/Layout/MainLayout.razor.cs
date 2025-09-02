using Microsoft.AspNetCore.Components;

namespace Server.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        bool sidebarExpanded = true;

        public void Dispose()
        {

        }
    }
}
