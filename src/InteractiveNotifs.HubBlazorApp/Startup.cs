using InteractiveNotifs.HubBlazorApp.ViewModels;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace InteractiveNotifs.HubBlazorApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddScoped<SendNotificationViewModel>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
