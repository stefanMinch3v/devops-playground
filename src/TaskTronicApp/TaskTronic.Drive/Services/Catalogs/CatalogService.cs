namespace TaskTronic.Drive.Services.Catalogs
{
    using Folders;
    using Microsoft.EntityFrameworkCore;
    using Models.Folders;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TaskTronic.Drive.Data;
    using TaskTronic.Drive.Data.Models;

    public class CatalogService : ICatalogService
    {
        private static readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        private readonly IFolderService folderService;
        private readonly DriveDbContext dbContext;

        public CatalogService(
            IFolderService folder,
            DriveDbContext dbContext)
        {
            this.folderService = folder;
            this.dbContext = dbContext;
        }

        public async Task<int> GetIdAsync(int companyDepartmentsId, int employeeId)
        {
            await locker.WaitAsync();

            try
            {
                var catalog = await this.dbContext.Catalogs
                    .FirstOrDefaultAsync(c => c.CompanyDepartmentsId == companyDepartmentsId);

                if (catalog is null)
                {
                    var newCatalog = new Catalog { CompanyDepartmentsId = companyDepartmentsId };

                    this.dbContext.Catalogs.Add(newCatalog);

                    await this.dbContext.SaveChangesAsync();

                    await this.folderService.CreateFolderAsync(this.CreateInputFolderModel(newCatalog.CatalogId, employeeId));

                    return newCatalog.CatalogId;
                }

                return catalog.CatalogId;
            }
            finally
            {
                locker.Release();
            }
        }

        private InputFolderServiceModel CreateInputFolderModel(int catId, int employeeId)
            => new InputFolderServiceModel
            {
                CatalogId = catId,
                EmployeeId = employeeId,
                Name = DriveConstants.ROOT_FOLDER_NAME,
                CreateDate = DateTime.UtcNow
            };
    }
}
