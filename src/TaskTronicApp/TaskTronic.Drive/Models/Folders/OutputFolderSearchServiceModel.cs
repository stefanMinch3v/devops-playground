namespace TaskTronic.Drive.Models.Folders
{
    using System.Collections.Generic;

    public class OutputFolderSearchServiceModel
    {
        public int FolderId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public bool IsPrivate { get; set; }

        public ICollection<OutputFolderSearchServiceModel> SubFolders { get; set; } = new List<OutputFolderSearchServiceModel>();

        public override string ToString()
        {
            return $"{this.FolderId} - {this.Name}";
        }
    }
}
