﻿using Microsoft.Extensions.Hosting;

namespace InterpolSystem.Web
{
    using AutoMapper;
    using Common.Mapping;
    using Data;
    using Data.Models;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class Startup
    {
        // user secrets
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder();

            builder.AddUserSecrets<Startup>();

            Configuration = builder.Build();

            foreach (var item in configuration.AsEnumerable())
            {
                Configuration[item.Key] = item.Value;
            }
        }

        // user secrets
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<InterpolDbContext>(options => options
                    .UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"])); // app secret

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<InterpolDbContext>()
            .AddDefaultTokenProviders();

            services.AddDomainServices(); // auto adds services

            services.AddAutoMapper(typeof(AutoMapperProfile)); // automapper assembly

            services.AddRouting(routing => routing.LowercaseUrls = true); // user friendly urls

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stats.
                options.ValidationInterval = TimeSpan.Zero;
            });

            services.AddHttpContextAccessor();

            services.AddControllersWithViews();

            services.AddAntiforgery(options =>
            {
                // Set Cookie properties using CookieBuilder properties†.
                options.FormFieldName = "AntiforgeryKey";
                options.HeaderName = "X-CSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });

            //services.AddMvc(options =>
            //{
            //    options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>(); // sets it global to all actions
            //});
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDatabaseMigration();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("missingpeople", "missingpeople/index", new { controller = "MissingPeople", action = "Index" });
                endpoints.MapControllerRoute("wantedpeople", "wantedpeople/index", new { controller = "WantedPeople", action = "Index" });
                endpoints.MapControllerRoute("blog", "blog/articles/{id}/{title}", new { area = "Blog", controller = "Articles", action = "Details" });
                endpoints.MapControllerRoute("default", "{controller=home}/{action=index}/{id?}");
            });

            app.UseCookiePolicy();
            
            //app.UseMvc(routes =>
            //{
            //    //routes.MapRoute(
            //    //    name: "wantedpeople",
            //    //    template: "wantedpeople/index",
            //    //    defaults: new { controller = "WantedPeople", action = "Index" });

            //    //routes.MapRoute(
            //    //    name: "missingpeople",
            //    //    template: "missingpeople/index",
            //    //    defaults: new { controller = "MissingPeople", action = "Index" });

            //    //routes.MapRoute(
            //    //    name: "blog",
            //    //    template: "blog/articles/{id}/{title}",
            //    //    defaults: new { area = "Blog", controller = "Articles", action = "Details" });

            //    //routes.MapRoute(
            //    //    name: "areas",
            //    //    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            //    //);

            //    //routes.MapRoute(
            //    //    name: "default",
            //    //    template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
