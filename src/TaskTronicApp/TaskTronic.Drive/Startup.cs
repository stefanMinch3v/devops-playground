namespace TaskTronic.Drive
{
    using Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services.Catalogs;
    using Services.CompanyDepartments;
    using Services.DapperRepo;
    using Services.Employees;
    using Services.Files;
    using Services.Folders;
    using Services.Messages;
    using System.Reflection;
    using TaskTronic.Drive.Messages;
    using TaskTronic.Infrastructure;
    using TaskTronic.Services;

    public class Startup
    {
        public Startup(IConfiguration configuration) 
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
            => services
                .AddApiService<DriveDbContext>(this.Configuration, "/api/files/DownloadFile")
                .AddSwaggerOptions(Assembly.GetExecutingAssembly().GetName().Name)
                .AddSingleton<IDbConnectionFactory, DbConnectionFactory>()
                .AddTransient<IDbSeeder, DriveDbSeeder>()
                .AddTransient<IFileService, FileService>()
                .AddTransient<IMessageService, MessageService>()
                .AddTransient<IEmployeeService, EmployeeService>()
                .AddTransient<ICatalogService, CatalogService>()
                .AddTransient<IFolderService, FolderService>()
                .AddTransient<ICompanyDepartmentsService, CompanyDepartmentsService>()
                .AddTransient<IPermissionDapper, PermissionDapper>()
                .AddTransient<IFileDapper, FileDapper>()
                .AddTransient<IFolderDapper, FolderDapper>()
                .AddMessaging(
                    useHangfireForPublishers: true,
                    configuration: this.Configuration,
                    typeof(UserRegisteredConsumer));

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            => app
                .UseApiService(env, addSwaggerUI: true)
                .Initialize();
    }
}
