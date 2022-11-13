namespace TaskTronic.Admin
{
    using Infrastructure;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Refit;
    using Services;
    using Services.Companies;
    using Services.Employees;
    using Services.Identity;
    using Services.Statistics;
    using System.Reflection;
    using TaskTronic.Infrastructure;
    using TaskTronic.Services.Identity;

    public class Startup
    {
        public Startup(IConfiguration configuration) 
            => this.Configuration = configuration;

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
                .AddTransient<JwtCookieAuthenticationMiddleware>()
                .AddControllersWithViews(options => options
                    .Filters.Add(new AutoValidateAntiforgeryTokenAttribute())); // global

            services
                .AddRefitClient<IIdentityService>()
                .WithConfiguration(serviceEndpoints.Identity);

            services
                .AddRefitClient<IStatisticsService>()
                .WithConfiguration(serviceEndpoints.Statistics);

            services
                .AddRefitClient<IEmployeeService>()
                .WithConfiguration(serviceEndpoints.Employees);

            services
                .AddRefitClient<ICompanyService>()
                .WithConfiguration(serviceEndpoints.Companies);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app
                    .UseDeveloperExceptionPage();
            }
            else
            {
                app
                    .UseExceptionHandler("/Home/Error")
                    .UseHsts();
            }

            app
                //.UseHttpsRedirection()
                .UseStaticFiles()
                .UseRouting()
                .UseJwtCookieAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints
                    .MapDefaultControllerRoute());
        }
    }
}
