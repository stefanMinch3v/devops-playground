namespace TaskTronic.Drive.Gateway.Models.Drive.Folders
{
    using TaskTronic.Models;

    public sealed class OutputMineFolderFlatModel : OutputFolderFlatServiceModel, IMapFrom<OutputFolderFlatServiceModel>
    {
        public int TotalViews { get; set; }
    }
}
