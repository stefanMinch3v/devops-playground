namespace TaskTronic.Drive.Services.Files
{
    using DapperRepo;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Spreadsheet;
    using Exceptions;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Models.Files;
    using Services.Folders;
    using Services.Messages;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using TaskTronic.Common;
    using TaskTronic.Data.Models;
    using TaskTronic.Drive.Data;
    using TaskTronic.Messages.Drive.Files;

    public class FileService : IFileService
    {
        private readonly IFileDapper fileDapper;
        private readonly IFolderService folderService;
        private readonly IMessageService messageService;
        private readonly IBus publisher;
        private readonly DriveDbContext dbContext;

        public FileService(
            IFileDapper fileDapper,
            IFolderService folderService,
            IMessageService messageService,
            IBus publisher,
            DriveDbContext dbContext)
        {
            this.fileDapper = fileDapper;
            this.folderService = folderService;
            this.messageService = messageService;
            this.publisher = publisher;
            this.dbContext = dbContext;
        }

        public async Task<bool> DeleteFileAsync(int employeeId, int catalogId, int folderId, int fileId)
        {
            await this.folderService.CheckFolderPermissionsAsync(catalogId, folderId, employeeId);

            var file = await this.dbContext.Files
                .FirstOrDefaultAsync(f => f.CatalogId == catalogId
                    && f.FolderId == folderId
                    && f.FileId == fileId);

            if (file is null)
            {
                throw new FileException { Message = "File not found." };
            }

            this.dbContext.Blobsdata.Remove(new Data.Models.Blobsdata { BlobId = file.BlobId });
            this.dbContext.Files.Remove(file);

            var messageData = new FileDeletedMessage
            {
                FileId = file.FileId
            };

            var message = new Message(messageData);

            this.dbContext.Messages.Add(message);

            await this.dbContext.SaveChangesAsync();

            await this.publisher.Publish(messageData);

            await this.messageService.MarkMessageAsPublishedAsync(message.Id);

            return true;
        }

        public async Task<bool> RenameFileAsync(int catalogId, int folderId, int fileId, int employeeId, string newFileName)
        {
            Guard.AgainstEmptyString<FileException>(newFileName, nameof(newFileName));
            Guard.AgainstInvalidWindowsCharacters<FileException>(newFileName, nameof(newFileName));

            await this.folderService.CheckFolderPermissionsAsync(catalogId, folderId, employeeId);

            var file = await this.dbContext.Files
                .FirstOrDefaultAsync(f => f.CatalogId == catalogId
                    && f.FolderId == folderId
                    && f.FileId == fileId);

            if (file is null)
            {
                throw new FileException { Message = "File not found." };
            }

            file.FileName = newFileName;

            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UploadFileAsync(InputFileServiceModel file)
        {
            Guard.AgainstNullObject<FileException>(file, nameof(file));
            Guard.AgainstEmptyString<FileException>(file.FileName, nameof(file.FileName));
            Guard.AgainstInvalidWindowsCharacters<FileException>(file.FileName, nameof(file.FileName));

            var firstChunk = file.Chunk == 0;
            var lastChunk = file.Chunk == file.Chunks - 1;

            // first chunk 
            if (firstChunk)
            {
                // create blob row
                var blobCreated = await this.fileDapper.CreateBlobAsync(file);

                if (file.Chunks > 1)
                {
                    return blobCreated;
                }
            }

            // append til get to the last chunk (middle)
            if (!firstChunk && !lastChunk)
            {
                return await this.fileDapper.AppendChunkToBlobAsync(file);
            }

            // if last chunk append and save file
            if (!firstChunk)
            {
                await this.fileDapper.AppendChunkToBlobAsync(file);
            }

            var insertedFileId = 0;
            var insertedMessageId = 0;

            var existingFileId = await this.fileDapper.DoesFileWithSameNameExistInFolder(
                file.CatalogId, 
                file.FolderId, 
                file.FileName, 
                file.FileType);

            file.CreateDate = DateTimeOffset.UtcNow;

            if (existingFileId > 0)
            {
                // file with name does exist in folder
                // add as new file with new fileName
                // check if we need to change filename
                var filesInFolder = await this.GetFilesByFolderIdAsync(file.CatalogId, file.FolderId, file.EmployeeId);

                if (filesInFolder.Any(f => f.FileName == file.FileName && f.FileType == file.FileType))
                {
                    var oldFileName = file.FileName;
                    this.RenameFileName(filesInFolder, file: file);

                    // TODO: refactor to EF and move messages out
                    var (FileId, MessageId) = await this.fileDapper.SaveCompletedUploadAsync(file, oldFileName);
                    insertedFileId = FileId;
                    insertedMessageId = MessageId;
                }

                if (insertedFileId < 1)
                {
                    return false;
                }
            }
            else // file with name does NOT exist in folder
            {
                // TODO: refactor to EF and move messages out
                var (FileId, MessageId) = await this.fileDapper.SaveCompletedUploadAsync(file);
                insertedFileId = FileId;
                insertedMessageId = MessageId;

                if (insertedFileId < 1)
                {
                    return false;
                }
            }

            if (insertedFileId > 0)
            {
                // TODO: refactor when move to EF
                var messageData = new FileUploadedMessage
                {
                    FileId = insertedFileId,
                    Name = file.FileName,
                    Type = file.ContentType
                };

                await this.publisher.Publish(messageData);

                await this.messageService.MarkMessageAsPublishedAsync(insertedMessageId);
            }

            return true;
        }

        public Task ReadStreamFromFileAsync(int blobId, Stream stream)
            => this.fileDapper.ReadStreamFromFileAsync(blobId, stream);

        public async Task<OutputFileDownloadServiceModel> GetFileInfoForDownloadAsync(
            int catalogId, 
            int employeeId, 
            int folderId, 
            int fileId)
        {
            await this.folderService.CheckFolderPermissionsAsync(catalogId, folderId, employeeId);
            return await this.fileDapper.GetFileInfoForDownloadAsync(fileId);
        }

        public async Task<bool> CreateNewFileAsync(int catalogId, int employeeId, int folderId, string fileName, NewFileType fileType)
            => fileType switch
            {
                NewFileType.Word => await this.CreateEmptyWordDocAsync(catalogId, employeeId, folderId, fileName),
                NewFileType.Excel => await this.CreateEmptyExcelDocAsync(catalogId, employeeId, folderId, fileName),
                _ => throw new InvalidOperationException($"Unsupported file type: {fileType}"),
            };

        private async Task<IEnumerable<IFileContract>> GetFilesByFolderIdAsync(
            int catalogId,
            int folderId,
            int employeeId)
        {
            await this.folderService.CheckFolderPermissionsAsync(catalogId, folderId, employeeId);
            return await this.fileDapper.GetFileNamesAndTypesAsync(folderId);
        }

        private void RenameFileName(
            IEnumerable<IFileContract> filesInFolder, 
            InputFileServiceModel file)
        {
            Guard.AgainstNullObject<FileException>(file, nameof(file));

            var nameExists = true;
            var name = file.FileName;

            while (nameExists)
            {
                // check if folder has " (x) "
                var match = Regex.Match(name, @"(.+)\((\d+)\)");

                // if not add (1)
                if (!match.Success)
                {
                    name += " (1)";
                }
                else
                {
                    // else add (x+1)
                    int.TryParse(match.Groups[2].Value, out int currentCount);

                    name = name.Replace($"({currentCount})", $"({currentCount + 1})");
                }

                if (!filesInFolder.Any(f => f.FileName == name && f.FileType == file.FileType))
                {
                    nameExists = false;
                    file.FileName = name;
                }
            }
        }

        private async Task<bool> CreateEmptyWordDocAsync(int catalogId, int employeeId, int folderId, string fileName)
        {
            using var ms = new MemoryStream();

            return await this.UploadFileAsync(new InputFileServiceModel
            {
                Chunk = 0,
                Chunks = 1,
                FileName = string.IsNullOrEmpty(fileName) ? "Document" : fileName,
                FileType = ".docx",
                Filesize = 0,
                CatalogId = catalogId,
                EmployeeId = employeeId,
                FolderId = folderId,
                ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                Stream = ms,
                CreateDate = DateTime.UtcNow
            });
        }

        private async Task<bool> CreateEmptyExcelDocAsync(int catalogId, int employeeId, int folderId, string fileName)
        {
            using var ms = this.GenerateEmptyExcelFileInStream();

            return await this.UploadFileAsync(new InputFileServiceModel
            {
                Chunk = 0,
                Chunks = 1,
                FileName = string.IsNullOrEmpty(fileName) ? "Excel" : fileName,
                FileType = ".xlsx",
                Filesize = ms.Length,
                CatalogId = catalogId,
                EmployeeId = employeeId,
                FolderId = folderId,
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Stream = ms,
                CreateDate = DateTime.UtcNow
            });
        }

        private Stream GenerateEmptyExcelFileInStream()
        {
            var ms = new MemoryStream();

            using (var spreadSheetDocument = SpreadsheetDocument.Create(ms, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workBookPart = spreadSheetDocument.AddWorkbookPart();
                workBookPart.Workbook = new Workbook();

                var workSheetPart = workBookPart.AddNewPart<WorksheetPart>();
                workSheetPart.Worksheet = new Worksheet(new SheetData());

                var sheets = spreadSheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                var sheet = new Sheet
                {
                    Id = spreadSheetDocument.WorkbookPart.GetIdOfPart(workSheetPart),
                    SheetId = 1,
                    Name = "WorkSheet1"
                };
                sheets.Append(sheet);
            }

            ms.Position = 0;
            return ms;
        }
    }
}
