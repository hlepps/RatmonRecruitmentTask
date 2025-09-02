using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Radzen;

namespace Server.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase, IDisposable
    {
        [CascadingParameter]
        private HttpContext HttpContext { get; set; }

        [Inject]
        private ThemeService ThemeService { get; set; }

        bool IsAccountRoute => NavigationManager.Uri.Contains("/Account/");
        bool sidebarExpanded = true;

        public void Dispose()
        {

        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (HttpContext != null)
            {
                var theme = HttpContext.Request.Cookies["ThemeService"];

                if (!string.IsNullOrEmpty(theme))
                {
                    ThemeService.SetTheme(theme, false);
                }
            }
        }

        private void OnHeaderClick()
        {
            NavigationManager.NavigateTo("/", forceLoad: true);
        }
    }
}
