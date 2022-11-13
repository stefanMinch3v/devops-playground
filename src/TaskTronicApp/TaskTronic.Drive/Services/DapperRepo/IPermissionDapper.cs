namespace TaskTronic.Drive.Services.DapperRepo
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPermissionDapper
    {
        Task CreateFolderPermissionsAsync(int catalogId, int folderId, int employeeId);

        Task<IEnumerable<int>> GetUserFolderPermissionsAsync(int catalogId, int employeeId);

        Task<bool> HasUserPermissionForFolderAsync(int catalogId, int folderId, int employeeId);
    }
}
