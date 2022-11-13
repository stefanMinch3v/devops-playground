namespace TaskTronic.Drive.Controllers
{
    using Common;
    using Exceptions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models.Files;
    using Services.Employees;
    using Services.Files;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using TaskTronic.Controllers;
    using TaskTronic.Services.Identity;

    [Authorize]
    public class FilesController : ApiController
    {
        private const int MAX_ALLOWED_FILE_SIZE = int.MaxValue; // The data column cannot hold more than 2^31-1 bytes (~2GB)
        private readonly IFileService fileService;
        private readonly IEmployeeService employeeService;
        private readonly ICurrentUserService currentUser;

        public FilesController(
            IFileService fileService, 
            IEmployeeService employeeService,
            ICurrentUserService currentUser)
        {
            this.fileService = fileService;
            this.employeeService = employeeService;
            this.currentUser = currentUser;
        }

        [HttpGet]
        [Route(nameof(DownloadFile))]
        public async Task<ActionResult> DownloadFile(int catId, int folderId, int fileId, bool shouldOpen)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            var downloadFile = await this.fileService.GetFileInfoForDownloadAsync(
                catId,
                employeeId, 
                folderId, 
                fileId);

            if (downloadFile is null)
            {
                return NotFound();
            }

            var dispositionAttribute = shouldOpen ? "inline" : "attachment";

            Response.Headers.Add(
                "Content-Disposition", 
                $"{dispositionAttribute}; filename={downloadFile.FileName}{downloadFile.FileType}");
            Response.Headers.Add("Content-Length", $"{downloadFile.FileSize}");
            Response.Headers.Add("Content-Type", $"{downloadFile.ContentType}");

            await this.fileService.ReadStreamFromFileAsync(downloadFile.BlobId, Response.Body);

            return new EmptyResult();
        }

        [HttpPost]
        [Route(nameof(RenameFile))]
        public async Task<ActionResult<bool>> RenameFile(int catalogId, int folderId, int fileId, string name)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            return await this.fileService.RenameFileAsync(catalogId, folderId, fileId, employeeId, name);
        }

        [HttpDelete]
        [Route(nameof(DeleteFile))]
        public async Task<ActionResult<bool>> DeleteFile(int catalogId, int folderId, int fileId)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            return await this.fileService.DeleteFileAsync(employeeId, catalogId, folderId, fileId);
        }

        [HttpPost]
        [Route(nameof(UploadFileToFolder))]
        public async Task<ActionResult<bool>> UploadFileToFolder(IFormFile file)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            if (file.Length > MAX_ALLOWED_FILE_SIZE)
            {
                throw new FileException { Message = $"The file is too large. {file.Length} exceeds the maximum size of {MAX_ALLOWED_FILE_SIZE}." };
            }

            var form = Request.Form;
            var extension = Path.GetExtension(form["name"]);
            var contentType = GetMimeType(extension);

            var model = this.GenerateInputFileModel(
                int.Parse(form["catalogId"]),
                int.Parse(form["folderId"]),
                contentType,
                extension,
                Path.GetFileNameWithoutExtension(form["name"]),
                employeeId,
                int.Parse(form["chunk"]),
                int.Parse(form["chunks"]),
                file.Length,
                file.OpenReadStream());

            return await this.fileService.UploadFileAsync(model);
        }

        [HttpPost]
        [Route(nameof(CreateNewFile))]
        public async Task<ActionResult<bool>> CreateNewFile(int catalogId, int folderId, string fileName, NewFileType newFileType)
        {
            var employeeId = await this.employeeService.GetIdByUserAsync(this.currentUser.UserId);

            if (employeeId == 0)
            {
                return BadRequest(DriveConstants.INVALID_EMPLOYEE);
            }

            return await this.fileService.CreateNewFileAsync(catalogId, employeeId, folderId, fileName, newFileType);
        }

        private static string GetMimeType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentNullException(nameof(extension));
            }

            if (!extension.StartsWith(".", StringComparison.Ordinal))
            {
                extension = "." + extension;
            }

            return MimeTypes.mappings.TryGetValue(extension, out string mime) ? mime : "application/octet-stream";
        }

        private InputFileServiceModel GenerateInputFileModel(
            int catId,
            int folderId,
            string contentType,
            string fileType,
            string name,
            int employeeId,
            int chunk,
            int chunks,
            long length,
            Stream openReadStream)
            => new InputFileServiceModel
            {
                CatalogId = catId,
                FolderId = folderId,
                ContentType = contentType,
                FileType = fileType,
                FileName = name,
                EmployeeId = employeeId,
                Chunk = chunk,
                Chunks = chunks,
                Filesize = length,
                Stream = openReadStream
            };
    }
}
