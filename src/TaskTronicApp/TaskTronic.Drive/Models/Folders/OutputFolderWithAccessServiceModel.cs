namespace TaskTronic.Drive.Models.Folders
{
    using System;

    public class OutputFolderWithAccessServiceModel
    {
        public int FolderId { get; set; }
        public int CatalogId { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public int? RootId { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public int EmployeeId { get; set; }
        public bool HasAccess { get; set; }
        public int FileCount { get; set; }
        public int FolderCount { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
