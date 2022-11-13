namespace TaskTronic.Drive.Gateway.Services.FolderViews
{
    using Models.FolderViews;
    using Refit;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFolderViewService
    {
        [Get("/FolderViews")]
        Task<IReadOnlyCollection<OutputFolderViewServiceModel>> TotalViews([Query(CollectionFormat.Multi)] IEnumerable<int> ids);
    }
}
