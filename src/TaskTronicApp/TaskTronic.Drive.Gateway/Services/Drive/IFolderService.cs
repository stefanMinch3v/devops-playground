namespace TaskTronic.Drive.Gateway.Services.Drive
{
    using Models.Drive.Folders;
    using Refit;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFolderService
    {
        [Get("/Folders/GetMine")]
        Task<IReadOnlyCollection<OutputFolderFlatServiceModel>> Mine();
    }
}
