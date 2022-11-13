namespace TaskTronic.Identity
{
    using Data;
    using Data.Models;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services.Identity;
    using Services.Jwt;
    using Services.Messages;
    using System.Reflection;
    using TaskTronic.Identity.Messages;
    using TaskTronic.Infrastructure;
    using TaskTronic.Services;

    public class Startup
    {
        public Startup(IConfiguration configuration) 
            => this.Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
            => services
                .AddApiService<IdentityDbContext>(this.Configuration)
                .AddSwaggerOptions(Assembly.GetExecutingAssembly().GetName().Name)
                .AddTransient<IDbSeeder, IdentityDbSeeder>()
                .AddTransient<IIdentityService, IdentityService>()
                .AddTransient<IMessageService, MessageService>()
                .AddTransient<IJwtGeneratorService, JwtGeneratorService>()
                .AddMessaging(
                    useHangfireForPublishers: true,
                    configuration: this.Configuration,
                    typeof(UserEditedConsumer))
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<IdentityDbContext>();
                

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            => app
                .UseApiService(env, addSwaggerUI: true)
                .Initialize();
    }
}
