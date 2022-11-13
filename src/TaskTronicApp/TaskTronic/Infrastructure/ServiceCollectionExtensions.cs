namespace TaskTronic.Infrastructure
{
    using AutoMapper;
    using GreenPipes;
    using Hangfire;
    using MassTransit;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using TaskTronic.Messages;
    using TaskTronic.Models;
    using TaskTronic.Services.Identity;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="allowedJwtInUrlAt">The url where jwt will be parsed from. Use it sparingly</param>
        /// <returns></returns>
        public static IServiceCollection AddApiService<TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string allowedJwtInUrlAt = null)
            where TDbContext : DbContext
        {
            services
                .AddDatabase<TDbContext>(configuration)
                .AddApplicationSettings(configuration)
                .AddTokenAuthentication(
                    configuration,
                    allowedJwtInUrlAt != null ? JwtConfiguration.BearerEvents(allowedJwtInUrlAt) : null)
                .AddAutoMapperProfile(Assembly.GetCallingAssembly())
                .AddCors()
                .AddHealth(configuration)
                .AddControllers();

            return services;
        }

        public static IServiceCollection AddDatabase<TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration)
            where TDbContext : DbContext
            => services
                .AddScoped<DbContext, TDbContext>()
                .AddDbContext<TDbContext>(options => options
                    .UseSqlServer(
                        configuration["ConnectionStrings:DefaultConnection"],
                        sqlServerOptions => sqlServerOptions
                            .EnableRetryOnFailure(
                                maxRetryCount: 10,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null)));

        public static IServiceCollection AddApplicationSettings(
            this IServiceCollection services,
            IConfiguration configuration)
            => services
                .Configure<ApplicationSettings>(
                    configuration.GetSection(nameof(ApplicationSettings)),
                    options => options.BindNonPublicProperties = true);

        public static IServiceCollection AddTokenAuthentication(
            this IServiceCollection services,
            IConfiguration configuration,
            JwtBearerEvents events = null)
        {
            var secret = configuration
                .GetSection(nameof(ApplicationSettings))
                .GetValue<string>(nameof(ApplicationSettings.Secret));

            var key = Encoding.ASCII.GetBytes(secret);

            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    // attaches the custom jwt event where we take the token from query
                    if (events != null)
                    {
                        bearer.Events = events;
                    }
                });

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }

        public static IServiceCollection AddAutoMapperProfile(
            this IServiceCollection services,
            Assembly assembly)
            => services
                .AddAutoMapper(
                    (_, config) => config.AddProfile(new MappingProfile(assembly)),
                    Array.Empty<Assembly>());

        public static IServiceCollection AddMessaging(
            this IServiceCollection services,
            bool useHangfireForPublishers = false,
            IConfiguration configuration = null,
            params Type[] consumers)
        {
            services
                .AddMassTransit(mt => // config the services before running
                {
                    consumers.ForEach(consumer => mt.AddConsumer(consumer));

                    mt.AddBus(busContext => Bus.Factory.CreateUsingRabbitMq(rmq =>
                    {
                        // should come from app config
                        rmq.Host("rabbitmq", host =>
                        {
                            host.Username("rabbitmq");
                            host.Password("rabbitmq");
                        });

                        rmq.UseHealthCheck(busContext);

                        consumers.ForEach(consumer => rmq.ReceiveEndpoint(consumer.FullName, endpoint =>
                        {
                            endpoint.PrefetchCount = (ushort)(Environment.ProcessorCount / 2); // number of CPUs to handle concurrently the messages
                            endpoint.UseMessageRetry(retry => retry.Interval(10, 1000));

                            endpoint.ConfigureConsumer(busContext, consumer);
                        }));
                    }));
                })
                .AddMassTransitHostedService(); // starts the services

            // make sense only for publishers cuz subscribers are supposed to recieve only
            if (useHangfireForPublishers)
            {
                if (configuration is null)
                {
                    throw new InvalidOperationException("Configuration is required for Hangfire.");
                }

                services
                    .AddHangfire(config => config
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(configuration["ConnectionStrings:DefaultConnection"]));

                services.AddHangfireServer();

                services.AddHostedService<MessagesHostedService>();
            }

            return services;
        }

        public static IServiceCollection AddHealth(
            this IServiceCollection services,
            IConfiguration configuration)

        {
            var healthChecks = services.AddHealthChecks();

            healthChecks.AddSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

            // host:username@password format
            healthChecks.AddRabbitMQ(rabbitConnectionString: "amqp://rabbitmq:rabbitmq@rabbitmq/");

            return services;
        }

        public static IServiceCollection AddSwaggerOptions(this IServiceCollection services, string assemblyName)
            => services
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(
                        "all",
                        new OpenApiInfo
                        {
                            Title = assemblyName,
                            Version = "All"
                        });

                    options.AddJwtToSwagger();

                    var xmlFile = $"{assemblyName}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);

                    options.CustomSchemaIds(type => type.FullName);
                });

        private static void AddJwtToSwagger(this SwaggerGenOptions swagger)
        {
            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = $"Authorization header using the 'Bearer' scheme.\n\rEnter 'Bearer' [space] and then your token in the text input below.\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "Bearer"
            });

            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        },
                    },
                    new List<string>()
                }
            });
        }
    }
}
