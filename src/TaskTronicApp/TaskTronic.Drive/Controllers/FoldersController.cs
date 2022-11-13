namespace TaskTronic.Drive.Controllers
{
    using Exceptions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Files;
    using Models.Folders;
    using Services.Catalogs;
    using Services.Employees;
    using Services.Files;
    using Services.Folders;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TaskTronic.Controllers;
    using TaskTronic.Services.Identity;

    [Authorize]
    public class FoldersController : ApiController
    {
        private readonly IFolderService folderService;
        private readonly ICatalogService catalogService;
        private readonly IFileService fileService;
        private readonly ICurrentUserService currentUser;
        private readonly IEmployeeService employeeService;

        public FoldersController(
            IFolderService folderService,
            ICatalogService catalogService,
            IFileService fileService,
            ICurrentUserService currentUser,
            IEmployeeService employeeService)
        {
            this.folderService = folderService;
            this.catalogService = catalogService;
            this.fileService = fileService;
            this.currentUser = currentUser;
            this.employeeService = employeeService;
        }

        [HttpPost]
        [Route(nameof(CreateFolder))]
        public async Task<ActionResult<bool>> CreateFolder(
            int catalogId,
            int rootId,
            int parentFolderId,
            string name,
            bool isPrivate = false)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            return await this.folderService.CreateFolderAsync(
                new InputFolderServiceModel
                {
                    CatalogId = catalogId,
                    Name = name,
                    ParentId = parentFolderId,
                    RootId = rootId,
                    IsPrivate = isPrivate,
                    EmployeeId = employeeId,
                    CreateDate = System.DateTime.UtcNow
                });
        }

        [HttpPost]
        [Route(nameof(RenameFolder))]
        public async Task<ActionResult<bool>> RenameFolder(int catalogId, int folderId, string name)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            try
            {
                return await this.folderService.RenameFolderAsync(catalogId, folderId, employeeId, name);
            }
            catch (FolderException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route(nameof(TogglePrivate))]
        public async Task<ActionResult> TogglePrivate(int catalogId, int folderId)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            try
            {
                await this.folderService.TogglePrivateAsync(catalogId, folderId, employeeId);
                return Ok();
            }
            catch (FolderException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route(nameof(GetRootFolder))]
        public async Task<ActionResult<OutputFolderServiceModel>> GetRootFolder(int companyDepartmentsId)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            var catalogId = await this.catalogService.GetIdAsync(companyDepartmentsId, employeeId);
            return await this.folderService.GetRootFolderByCatalogIdAsync(catalogId, employeeId);
        }

        [HttpGet]
        [Route(nameof(GetFolderById))]
        public async Task<ActionResult<OutputFolderServiceModel>> GetFolderById(int folderId)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            try
            {
                return await this.folderService.GetFolderByIdAsync(folderId, employeeId);
            }
            catch (FolderException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route(nameof(DeleteFolder))]
        public async Task<ActionResult<bool>> DeleteFolder(int catalogId, int folderId)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            return await this.folderService.DeleteFolderAsync(catalogId, employeeId, folderId);
        }

        [HttpGet]
        [Route(nameof(GetMine))]
        public async Task<ActionResult<IReadOnlyCollection<OutputFolderFlatServiceModel>>> GetMine()
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            return Ok(await this.folderService.GetAllForEmployeeAsync(employeeId));
        }

        [HttpGet]
        [Route(nameof(SearchForFiles))]
        public async Task<ActionResult<IReadOnlyCollection<OutputFileSearchServiceModel>>> SearchForFiles(
            int catalogId,
            int currentFolderId,
            string searchValue)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            return Ok(await this.folderService.SearchFilesAsync(catalogId, employeeId, currentFolderId, searchValue));
        }
    }
}
