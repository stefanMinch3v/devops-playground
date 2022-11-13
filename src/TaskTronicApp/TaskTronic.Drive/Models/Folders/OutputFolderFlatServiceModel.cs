namespace TaskTronic.Drive.Models.Folders
{
    using System;

    public class OutputFolderFlatServiceModel
    {
        public int FolderId { get; set; }

        public string Name { get; set; }

        public bool IsPrivate { get; set; }

        public DateTimeOffset CreateDate { get; set; }
    }
}
