namespace TaskTronic.Drive.Services.DapperRepo
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TaskTronic.Drive.Models.Folders;

    public interface IFolderDapper
    {
        Task<OutputFolderServiceModel> GetFolderByIdAsync(int folderId);

        Task<OutputFolderCatalogServiceModel> GetFolderCatalogAsync(int folderId);

        Task<OutputFolderFlatServiceModel> GetFolderFlatByIdAsync(int folderId);

        Task<OutputFolderServiceModel> GetRootFolderByCatalogIdAsync(int catalogId);

        Task<int?> GetRootFolderIdAsync(int folderId);

        Task<IEnumerable<OutputFolderServiceModel>> GetSubFoldersAsync(int folderId);

        Task<bool> IsFolderPrivateAsync(int folderId);

        Task<IEnumerable<OutputFolderWithAccessServiceModel>> GetFolderTreeAsync(int folderId, int employeeId);

        Task<int> GetFolderNumbersWithExistingNameAsync(string name, int parentFolderId);

        Task<int> CountFoldersForEmployeeAsync(int employeeId);

        Task<IReadOnlyCollection<OutputFolderFlatServiceModel>> GetAllFlatForEmployeeAsync(int employeeId);

        Task<IEnumerable<OutputFolderSearchServiceModel>> GetAllForSearchAsync(int catalogId, int? rootFolderId);

        // transactions
        Task<(bool Success, int MessageId)> DeleteAsync(int catalogId, int folderId);
        Task<(int FolderId, int MessageId)> CreateFolderAsync(InputFolderServiceModel inputModel);
    }
}
