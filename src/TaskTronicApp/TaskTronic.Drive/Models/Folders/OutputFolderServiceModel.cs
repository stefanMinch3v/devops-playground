namespace TaskTronic.Drive.Models.Folders
{
    using Models.Files;
    using System;
    using System.Collections.Generic;

    public class OutputFolderServiceModel
    {
        public int FolderId { get; set; }
        public int CatalogId { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public int? RootId { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public int EmployeeId { get; set; }
        public int FileCount { get; set; }
        public int FolderCount { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string CreatorUsername { get; set; }

		// for breadcrumbs navigation
        public OutputFolderServiceModel ParentFolder { get; set; }

        public ICollection<OutputFileServiceModel> Files { get; set; } = new List<OutputFileServiceModel>();
        public ICollection<OutputFolderServiceModel> SubFolders { get; set; } = new List<OutputFolderServiceModel>();
    }
}
