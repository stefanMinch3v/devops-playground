namespace TaskTronic.Drive.Services.Folders
{
    using DapperRepo;
    using Exceptions;
    using MassTransit;
    using Microsoft.EntityFrameworkCore;
    using Models.Files;
    using Models.Folders;
    using Services.Employees;
    using Services.Messages;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using TaskTronic.Common;
    using TaskTronic.Data.Models;
    using TaskTronic.Drive.Data;
    using TaskTronic.Messages.Drive.Folders;

    public class FolderService : IFolderService
    {
        private readonly IFolderDapper folderDapper;
        private readonly IPermissionDapper permissionsDapper;
        private readonly IFileDapper fileDapper;
        private readonly IEmployeeService employeeService;
        private readonly IMessageService messageService;
        private readonly IBus publisher;
        private readonly DriveDbContext dbContext;

        public FolderService(
            IFolderDapper folderDapper,
            IPermissionDapper permissionsDapper,
            IFileDapper fileDapper,
            IEmployeeService employeeService,
            IMessageService messageService,
            IBus publisher,
            DriveDbContext dbContext)
        {
            this.folderDapper = folderDapper;
            this.permissionsDapper = permissionsDapper;
            this.fileDapper = fileDapper;
            this.employeeService = employeeService;
            this.messageService = messageService;
            this.publisher = publisher;
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateFolderAsync(InputFolderServiceModel inputModel)
        {
            this.ValidateInput(inputModel);
            await this.ValidateParentAndRootAsync(inputModel);

            if (inputModel.Name != DriveConstants.ROOT_FOLDER_NAME)
            {
                // TODO: fix (needs to be fixed cuz the names are sometimes incorrect)
                var numberOfExistingNames = await folderDapper.GetFolderNumbersWithExistingNameAsync(
                    inputModel.Name,
                    inputModel.ParentId.Value);

                if (numberOfExistingNames > 0)
                {
                    inputModel.Name += $" ({++numberOfExistingNames})";
                }
            }

            var (FolderId, MessageId) = await folderDapper.CreateFolderAsync(inputModel);

            if (FolderId > 0 && inputModel.IsPrivate)
            {
                await this.permissionsDapper.CreateFolderPermissionsAsync(inputModel.CatalogId, FolderId, inputModel.EmployeeId);
            }

            // send message to the subscribers
            if (FolderId > 0)
            {
                var messageData = new FolderCreatedMessage
                {
                    FolderId = FolderId
                };

                await this.publisher.Publish(messageData);

                await this.messageService.MarkMessageAsPublishedAsync(MessageId);
            }

            return FolderId > 0;
        }

        public async Task<bool> RenameFolderAsync(int catalogId, int folderId, int employeeId, string newFolderName)
        {
            Guard.AgainstEmptyString<FolderException>(newFolderName, nameof(newFolderName));
            Guard.AgainstInvalidWindowsCharacters<FolderException>(newFolderName, newFolderName);

            await this.CheckFolderPermissionsAsync(catalogId, folderId, employeeId);

            var folder = await this.dbContext.Folders.FindAsync(folderId);

            if (folder is null)
            {
                return false;
            }

            if (folder.ParentId is null)
            {
                throw new FolderException { Message = "Cannot rename Root folder" };
            }

            folder.Name = newFolderName;

            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<OutputFolderServiceModel> GetFolderByIdAsync(
            int folderId, 
            int employeeId, 
            int? companyDepartmentsId = null)
        {
            if (companyDepartmentsId.HasValue)
            {
                var selectedDataId = await this.employeeService.GetSelectedCompanyDepartmentId(employeeId);

                if (selectedDataId != companyDepartmentsId.Value)
                {
                    throw new FolderException { Message = "The selected company/department is invalid." };
                }
            }

            var folder = await this.folderDapper.GetFolderByIdAsync(folderId);
            Guard.AgainstNullObject<FolderException>(folder, nameof(folder));

            var rootId = folder.RootId;

            if (rootId is null)
            {
                rootId = folder.FolderId;
            }

            var folderToReturn = await this.GenerateFolderWithTreeAsync(rootId.Value, employeeId, folderId);

            folderToReturn.Files = (await fileDapper.GetFilesByFolderIdAsync(folderId)).ToArray();

            foreach (var file in folderToReturn.Files)
            {
                file.CreatorUsername = await this.employeeService.GetEmailByIdAsync(file.EmployeeId);
            }

            folder.CreatorUsername = await this.employeeService.GetEmailByIdAsync(folder.EmployeeId);

            // save and send message
            var messageData = new FolderOpenedMessage
            {
                FolderId = folder.FolderId,
                UserId = await this.employeeService.GetUserIdByEmployeeAsync(employeeId)
            };

            var message = new Message(messageData);

            await this.messageService.SaveAsync(message);

            await this.publisher.Publish(messageData);

            await this.messageService.MarkMessageAsPublishedAsync(message.Id);

            return folderToReturn;
        }

        public Task<bool> IsFolderPrivateAsync(int folderId)
            => folderDapper.IsFolderPrivateAsync(folderId);

        public async Task<OutputFolderServiceModel> GetRootFolderByCatalogIdAsync(
            int catalogId, 
            int employeeId,
            bool includeSubfolders = true)
        {
            var root = await this.folderDapper.GetRootFolderByCatalogIdAsync(catalogId);

            if (root is null)
            {
                return null;
            }

            OutputFolderServiceModel folder;

            if (includeSubfolders)
            {
                folder = await this.GenerateFolderWithTreeAsync(root.FolderId, employeeId, root.FolderId);
            }
            else
            {
                folder = root;
            }

            folder.Files = (await this.fileDapper.GetFilesByFolderIdAsync(folder.FolderId)).ToArray();

            foreach (var file in folder.Files)
            {
                file.CreatorUsername = await this.employeeService.GetEmailByIdAsync(file.EmployeeId);
            }

            folder.CreatorUsername = await this.employeeService.GetEmailByIdAsync(folder.EmployeeId);

            var messageData = new FolderOpenedMessage
            {
                FolderId = folder.FolderId,
                UserId = await this.employeeService.GetUserIdByEmployeeAsync(employeeId)
            };

            var message = new Message(messageData);

            await this.messageService.SaveAsync(message);

            await this.publisher.Publish(messageData);

            await this.messageService.MarkMessageAsPublishedAsync(message.Id);

            return folder;
        }

        public async Task<bool> DeleteFolderAsync(int catalogId, int employeeId, int folderId)
        {
            var existingFolder = await this.folderDapper.GetFolderByIdAsync(folderId);
            return await this.DeleteFolderOperationsAsync(catalogId, employeeId, existingFolder);
        }

        public Task<IReadOnlyCollection<OutputFolderFlatServiceModel>> GetAllForEmployeeAsync(int employeeId)
            => this.folderDapper.GetAllFlatForEmployeeAsync(employeeId);

        public async Task<IReadOnlyCollection<OutputFileSearchServiceModel>> SearchFilesAsync(
            int catalogId,
            int employeeId, 
            int currentFolderId, 
            string searchValue)
        {
            var rootFolderId = await this.folderDapper.GetRootFolderIdAsync(currentFolderId);
            if (rootFolderId is null)
            {
                rootFolderId = currentFolderId;
            }

            var folders = await this.folderDapper.GetAllForSearchAsync(catalogId, rootFolderId);
            if (!folders.Any())
            {
                return Array.Empty<OutputFileSearchServiceModel>();
            }

            var currentFolder = folders.FirstOrDefault(f => f.FolderId == currentFolderId);
            if (currentFolder is null)
            {
                return Array.Empty<OutputFileSearchServiceModel>();
            }

            var folderTree = await this.AttachSubFoldersForSearchAsync(catalogId, employeeId, currentFolder, folders);

            var accessibleFolderIdsToLookAt = this.GetFolderIdsFromCurrentFolder(folderTree, new List<int>())
                .Distinct();

            var foundFiles = await this.fileDapper.SearchFilesAsync(
                catalogId, 
                searchValue,
                accessibleFolderIdsToLookAt);

            this.AddFolerNameToSearchFiles(foundFiles, folders);

            await this.AddUpdatersUsernamesAsync(foundFiles);

            return foundFiles
                .OrderByDescending(f => f.CreateDate)
                .ThenBy(f => f.SearchFolderNamesPath.Count)
                .ToArray();
        }

        public async Task CheckFolderPermissionsAsync(int catalogId, int folderId, int employeeId)
        {
            var folder = await this.folderDapper.GetFolderFlatByIdAsync(folderId);
            Guard.AgainstNullObject<FolderException>(folder, nameof(folder));

            if (folder.IsPrivate)
            {
                var hasPermission = await this.permissionsDapper.HasUserPermissionForFolderAsync(
                    catalogId,
                    folderId,
                    employeeId);

                if (!hasPermission)
                {
                    throw new PermissionException { Message = "You dont have access to this folder." };
                }
            }
        }

        public async Task TogglePrivateAsync(int catalogId, int folderId, int employeeId)
        {
            var folder = await this.dbContext.Folders
                .FirstOrDefaultAsync(f => f.CatalogId == catalogId
                    && f.FolderId == folderId
                    && f.EmployeeId == employeeId);

            if (folder is null)
            {
                throw new FolderException { Message = "Cannot make other people's folder private" };
            }

            if (folder.RootId is null)
            {
                throw new FolderException { Message = "Cannot make Root folder private" };
            }

            folder.IsPrivate = !folder.IsPrivate;

            if (folder.IsPrivate)
            {
                this.dbContext.Permissions.Add(new Data.Models.Permission
                {
                    CatalogId = catalogId,
                    FolderId = folderId,
                    EmployeeId = employeeId
                });
            } 
            else
            {
                var permission = await this.dbContext.Permissions
                    .FirstOrDefaultAsync(p => p.EmployeeId == employeeId
                        && p.FolderId == folderId
                        && p.CatalogId == catalogId);

                if (permission != null)
                {
                    this.dbContext.Permissions.Remove(permission);
                }
            }

            await this.dbContext.SaveChangesAsync();
        }

        // todo same thing as file regex
        //private void RenameFolderName(dynamic folder, dynamic newFolder)
        //{
        //    var nameExists = true;
        //    var name = folder.Name;

        //    while (nameExists)
        //    {
        //        // check if folder has " (x) "
        //        var match = Regex.Match(name, @"(.+)\((\d+)\)");

        //        // if not add (1)
        //        if (!match.Success)
        //        {
        //            name += " (1)";
        //        }
        //        else
        //        {
        //            // else add (x+1)
        //            int.TryParse(match.Groups[2].Value, out int currentCount);

        //            name = name.Replace($"({currentCount})", $"({currentCount + 1})");
        //        }

        //        if (!newFolder.SubFolders.Any(f => f.Name.Equals(name)))
        //        {
        //            nameExists = false;
        //            folder.Name = name;
        //        }
        //    }
        //}

        private async Task AddUpdatersUsernamesAsync(IEnumerable<OutputFileSearchServiceModel> foundFiles)
        {
            foreach (var file in foundFiles)
            {
                file.UpdaterUsername = await this.employeeService.GetEmailByIdAsync(file.EmployeeId);
            }
        }

        private async Task<OutputFolderSearchServiceModel> AttachSubFoldersForSearchAsync(
            int catalogId,
            int userId,
            OutputFolderSearchServiceModel currentFolder,
            IEnumerable<OutputFolderSearchServiceModel> allFolders)
        {
            foreach (var subFolder in allFolders.Where(f => f.ParentId == currentFolder.FolderId))
            {
                if (subFolder.IsPrivate)
                {
                    var hasPermission = this.dbContext.Permissions
                        .Any(p => p.FolderId == subFolder.FolderId && p.EmployeeId == userId);

                    if (!hasPermission)
                    {
                        continue;
                    }
                }

                currentFolder.SubFolders.Add(subFolder);
            }

            var userPermissions = await this.permissionsDapper.GetUserFolderPermissionsAsync(catalogId, userId);

            await this.AttachSubFoldersRecursivelyForSearchAsync(
                catalogId, 
                userId, 
                currentFolder.SubFolders,
                allFolders,
                userPermissions);

            return currentFolder;
        }

        private async Task AttachSubFoldersRecursivelyForSearchAsync(
            int catalogId,
            int userId,
            ICollection<OutputFolderSearchServiceModel> folders,
            IEnumerable<OutputFolderSearchServiceModel> allFolders,
            IEnumerable<int> userPermissions)
        {
            foreach (var folder in folders)
            {
                if (folder.IsPrivate)
                {
                    if (!userPermissions.Any())
                    {
                        continue;
                    }

                    var hasPermission = userPermissions.Any(p => p == folder.FolderId);
                    if (!hasPermission)
                    {
                        continue;
                    }
                }

                folder.SubFolders = allFolders
                    .Where(f => f.ParentId == folder.FolderId)
                    .ToList();

                await this.AttachSubFoldersRecursivelyForSearchAsync(
                    catalogId, 
                    userId, 
                    folder.SubFolders, 
                    allFolders, 
                    userPermissions);
            }
        }

        private List<int> GetFolderIdsFromCurrentFolder(OutputFolderSearchServiceModel folder, List<int> savedIds)
        {
            if (folder is null)
            {
                return savedIds;
            }

            savedIds.Add(folder.FolderId);

            foreach (var subFolder in folder.SubFolders)
            {
                savedIds.Add(subFolder.FolderId);
                savedIds = this.GetFolderIdsFromCurrentFolder(subFolder, savedIds);
            }

            return savedIds;
        }

        private void AddFolerNameToSearchFiles(
            IEnumerable<OutputFileSearchServiceModel> files, 
            IEnumerable<OutputFolderSearchServiceModel> folders)
        {
            foreach (var file in files)
            {
                var folderPathList = new List<string>();

                var folder = folders.FirstOrDefault(f => f.FolderId == file.FolderId);
                while (folder.ParentId != null)
                {
                    folderPathList.Add(folder.Name);
                    folder = folders.FirstOrDefault(f => f.FolderId == folder.ParentId);
                }

                folderPathList.Add(folder?.Name);
                folderPathList.Reverse();
                file.SearchFolderNamesPath = folderPathList;
            }
        }

        private async Task<bool> DeleteFolderOperationsAsync(int catalogId, int employeeId, OutputFolderServiceModel existingFolder)
        {
            try
            {
                await this.CheckFolderPermissionsAsync(catalogId, existingFolder?.FolderId ?? 0, employeeId);
                await this.AttachSubFoldersAsync(catalogId, employeeId, existingFolder);
                await this.DeleteFilesAsync(catalogId, employeeId, existingFolder);
                await this.DeleteFolderAndSubFoldersRecursivelyAsync(catalogId, employeeId, existingFolder);

                return true;
            }
            catch (FolderException)
            {
                throw;
            }
            catch (PermissionException)
            {
                throw;
            }
            catch
            {
                return false;
            }
        }

        private async Task DeleteFolderAndSubFoldersRecursivelyAsync(int catalogId, int employeeId, OutputFolderServiceModel folder)
        {
            // Cant delete the root folder
            if (folder is null || !folder.RootId.HasValue)
            {
                return;
            }

            // folders must be empty at this point
            var (isDeleted, insertedMessageId) = await this.folderDapper.DeleteAsync(catalogId, folder.FolderId);
            if (!isDeleted)
            {
                return;
            }

            // TODO: refactor when move to EF
            var messageData = new FolderDeletedMessage
            {
                FolderId = folder.FolderId
            };

            await this.publisher.Publish(messageData);

            await this.messageService.MarkMessageAsPublishedAsync(insertedMessageId);


            foreach (var subFolder in folder.SubFolders)
            {
                await this.DeleteFolderAndSubFoldersRecursivelyAsync(catalogId, employeeId, subFolder);
            }
        }

        private async Task AttachSubFoldersAsync(int catalogId, int employeeId, OutputFolderServiceModel folder)
        {
            folder.SubFolders = (await this.folderDapper.GetSubFoldersAsync(folder.FolderId)).ToArray();
            await this.AttachSubFoldersRecursivelyAsync(catalogId, employeeId, folder.SubFolders);
        }

        private async Task AttachSubFoldersRecursivelyAsync(int catalogId, int employeeId, IEnumerable<OutputFolderServiceModel> folders)
        {
            foreach (var folder in folders)
            {
                // skip if you dont have access
                if (folder.IsPrivate)
                {
                    var hasPermission = await this.permissionsDapper.HasUserPermissionForFolderAsync(
                        catalogId, 
                        folder.FolderId, 
                        employeeId);

                    if (!hasPermission)
                    {
                        continue;
                    }
                }

                folder.SubFolders = (await this.folderDapper.GetSubFoldersAsync(folder.FolderId)).ToArray();
                await this.AttachSubFoldersRecursivelyAsync(catalogId, employeeId, folder.SubFolders);
            }
        }

        private async Task DeleteFilesAsync(int catalogId, int employeeId, OutputFolderServiceModel parentFolder)
        {
            if (parentFolder is null)
            {
                return;
            }

            await this.DeleteFilesForFolderAsync(parentFolder.FolderId);

            foreach (var subFolder in parentFolder.SubFolders)
            {
                await this.DeleteFilesForFolderAsync(subFolder.FolderId);
                await this.DeleteFilesAsync(catalogId, employeeId, subFolder);
            }
        }

        private async Task DeleteFilesForFolderAsync(int folderId)
        {
            var files = await this.fileDapper.GetFilesByFolderIdAsync(folderId);

            foreach (var file in files)
            {
                await this.fileDapper.DeleteFileAsync(file.CatalogId, file.FolderId, file.FileId, file.BlobId);
            }
        }

        private void ValidateInput(InputFolderServiceModel model)
        {
            Guard.AgainstNullObject<FolderException>(model, nameof(model));
            Guard.AgainstEmptyString<FolderException>(model.Name, nameof(model.Name));
            Guard.AgainstInvalidWindowsCharacters<FolderException>(model.Name, nameof(model.Name));
            Guard.AgainstLessThanOne<FolderException>(model.CatalogId, nameof(model.CatalogId));
            Guard.AgainstLessThanOne<FolderException>(model.EmployeeId, nameof(model.EmployeeId));

            if (model.IsPrivate && (model.RootId is null))
            {
                throw new FolderException { Message = "The folder is private." };
            }

            if (model.CreateDate == default)
            {
                throw new FolderException { Message = "DateTime is required." };
            }
        }

        private async Task ValidateParentAndRootAsync(InputFolderServiceModel model)
        {
            if (model.ParentId != null && model.RootId != null)
            {
                var parentModelTask = this.folderDapper.GetFolderCatalogAsync(model.ParentId.Value);
                var rootModelTask = this.folderDapper.GetFolderCatalogAsync(model.RootId.Value);

                await Task.WhenAll(parentModelTask, rootModelTask);

                var parentModel = await parentModelTask;
                var rootModel = await rootModelTask;

                if (parentModel is null)
                {
                    throw new FolderException { Message = "The parent folder does not exist." };
                }

                if (rootModel is null)
                {
                    throw new FolderException { Message = "The root folder does not exist" };
                }

                if (parentModel.CatalogId != model.CatalogId || rootModel.CatalogId != model.CatalogId)
                {
                    throw new FolderException { Message = "Different catalogs." };
                }
            }
        }

        private OutputFolderServiceModel MapToOutputFolderServiceModel(OutputFolderWithAccessServiceModel folder)
        {
            if (folder is null)
            {
                return null;
            }

            return new OutputFolderServiceModel
            {
                CatalogId = folder.CatalogId,
                EmployeeId = folder.EmployeeId,
                FolderId = folder.FolderId,
                IsPrivate = folder.IsPrivate,
                Name = folder.Name,
                ParentId = folder.ParentId,
                ParentName = folder.ParentName,
                RootId = folder.RootId,
                FileCount = folder.FileCount,
                FolderCount = folder.FolderCount,
                CreateDate = folder.CreateDate
            };
        }

        private void CalculateTotalFileCountAndFolderCountForTree(OutputFolderServiceModel folder)
        {
            if (folder.SubFolders.Any())
            {
                foreach (var subFolder in folder.SubFolders)
                {
                    CalculateTotalFileCountAndFolderCountForTree(subFolder);
                }

                folder.FileCount += folder.SubFolders.Sum(f => f.FileCount);
                folder.FolderCount += folder.SubFolders.Sum(f => f.FolderCount);
            }
        }

        private async Task<OutputFolderServiceModel> GenerateFolderWithTreeAsync(int rootFolderId, int employeeId, int folderId)
        {
            var folderTree = await this.folderDapper.GetFolderTreeAsync(rootFolderId, employeeId);
            var rootTree = this.MapToOutputFolderServiceModel(folderTree.First(f => f.ParentId is null && f.RootId is null));

            var foldersToFindSubfoldersFor = new List<OutputFolderServiceModel> { rootTree };

            while (foldersToFindSubfoldersFor.Any())
            {
                var folder = foldersToFindSubfoldersFor.First();
                folder.SubFolders = folderTree
                    .Where(f => f.HasAccess && f.ParentId == folder.FolderId)
                    .Select(this.MapToOutputFolderServiceModel)
                    .ToList();

                foldersToFindSubfoldersFor.Remove(folder);
                foldersToFindSubfoldersFor.AddRange(folder.SubFolders);
            }

            this.CalculateTotalFileCountAndFolderCountForTree(rootTree);

            var folderToReturn = this.FindFolderWithId(rootTree, folderId);

            folderToReturn.Files = (await this.fileDapper.GetFilesByFolderIdAsync(folderToReturn.FolderId)).ToArray();

            foreach (var subfolder in folderToReturn.SubFolders)
            {
                subfolder.CreatorUsername = await this.employeeService.GetEmailByIdAsync(subfolder.EmployeeId);
            }

            this.AddParentFolderRecursively(folderTree, folderToReturn);

            return folderToReturn;
        }

        private OutputFolderServiceModel FindFolderWithId(OutputFolderServiceModel folder, int folderId)
        {
            if (folder.FolderId == folderId)
            {
                return folder;
            }
            else if (!folder.SubFolders.Any())
            {
                return null;
            }
            else
            {
                foreach (var subFolder in folder.SubFolders)
                {
                    var foundFolder = this.FindFolderWithId(subFolder, folderId);
                    if (foundFolder != null)
                    {
                        return foundFolder;
                    }
                }

                return null;
            }
        }

        // need for the breadcrumbs navigation
        private void AddParentFolderRecursively(
            IEnumerable<OutputFolderWithAccessServiceModel> folders, 
            OutputFolderServiceModel folder)
        {
            if (folder.ParentId.HasValue)
            {
                folder.ParentFolder = this.MapToOutputFolderServiceModel(
                    folders.FirstOrDefault(x => x.FolderId == folder.ParentId.Value));

                this.AddParentFolderRecursively(folders, folder.ParentFolder);
            }
        }
    }
}
