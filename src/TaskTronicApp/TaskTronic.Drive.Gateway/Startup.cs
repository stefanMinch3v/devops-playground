namespace TaskTronic.Drive.Gateway
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Refit;
    using Services;
    using System.Reflection;
    using TaskTronic.Drive.Gateway.Services.Drive;
    using TaskTronic.Drive.Gateway.Services.FolderViews;
    using TaskTronic.Infrastructure;
    using TaskTronic.Services.Identity;

    public class Startup
    {
        public Startup(IConfiguration configuration) 
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var serviceEndpoints = this.Configuration
                .GetSection(nameof(ServiceEndpoints))
                .Get<ServiceEndpoints>(config => config.BindNonPublicProperties = true);

            services
                .AddAutoMapperProfile(Assembly.GetExecutingAssembly())
                .AddTokenAuthentication(this.Configuration)
                .AddScoped<ICurrentTokenService, CurrentTokenService>()
                .AddTransient<JwtHeaderAuthenticationMiddleware>()
                .AddCors()
                .AddSwaggerOptions(Assembly.GetExecutingAssembly().GetName().Name)
                .AddControllers();

            services
                .AddRefitClient<IFolderService>()
                .WithConfiguration(serviceEndpoints.Drive);

            services
                .AddRefitClient<IFolderViewService>()
                .WithConfiguration(serviceEndpoints.Statistics);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            => app
                .UseGatewayApiService(env, addSwaggerUI: true);
    }
}
