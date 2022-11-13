namespace TaskTronic.Statistics.Services.FolderViews
{
    using Models.FolderViews;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFolderViewService
    {
        Task<int> GetTotalViews(int folderId);

        Task<IReadOnlyCollection<OutputFolderViewServiceModel>> GetTotalViews(IEnumerable<int> ids);

        Task AddViewAsync(int folderId, string userId);
    }
}
