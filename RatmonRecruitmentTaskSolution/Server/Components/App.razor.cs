using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Server.Components
{
    public partial class App : ComponentBase
    {
        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        [Inject]
        private ThemeService? ThemeService { get; set; }

        private IComponentRenderMode? RenderModeForPage => HttpContext.Request.Path.StartsWithSegments("/Account")
            ? null
            : RenderMode.InteractiveServer;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (HttpContext != null)
            {
                string? theme = HttpContext.Request.Cookies["ApplicationTheme"];

                if (!string.IsNullOrEmpty(theme) && ThemeService is not null)
                {
                    ThemeService.SetTheme(theme, false);
                }
            }
        }
    }
}
