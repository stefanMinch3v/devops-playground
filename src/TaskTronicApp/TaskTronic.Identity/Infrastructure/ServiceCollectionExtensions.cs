namespace TaskTronic.Identity.Infrastructure
{
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    // does not work for some reason, TODO: try to fix it after the gateway task
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultUserStorage(
            this IServiceCollection services)
        {
            services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<IdentityDbContext>();

            return services;
        }
    }
}
