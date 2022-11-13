namespace TaskTronic.Watchdog
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
            => services
                .AddHealthChecksUI()
                .AddInMemoryStorage();

        // default url: /healthchecks-ui
        public void Configure(IApplicationBuilder app)
            => app
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapHealthChecksUI());
    }
}
