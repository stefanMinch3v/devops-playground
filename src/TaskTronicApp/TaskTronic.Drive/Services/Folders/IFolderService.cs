namespace TaskTronic.Drive.Services.Folders
{
    using Models.Files;
    using Models.Folders;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFolderService
    {
        Task<bool> CreateFolderAsync(InputFolderServiceModel inputModel);

        Task<bool> RenameFolderAsync(int catalogId, int folderId, int employeeId, string newFolderName);

        Task<OutputFolderServiceModel> GetFolderByIdAsync(int folderId, int employeeId, int? companyDepartmentsId = null);

        Task CheckFolderPermissionsAsync(int catalogId, int folderId, int employeeId);

        Task<OutputFolderServiceModel> GetRootFolderByCatalogIdAsync(
            int catalogId, 
            int employeeId,
            bool includeSubfolders = true);

        Task<bool> DeleteFolderAsync(int catalogId, int employeeId, int folderId);

        Task<bool> IsFolderPrivateAsync(int folderId);

        Task<IReadOnlyCollection<OutputFolderFlatServiceModel>> GetAllForEmployeeAsync(int employeeId);

        Task<IReadOnlyCollection<OutputFileSearchServiceModel>> SearchFilesAsync(
            int catalogId,
            int employeeId,
            int currentFolderId,
            string searchValue);

        Task TogglePrivateAsync(int catalogId, int folderId, int employeeId);
    }
}
