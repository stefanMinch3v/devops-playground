namespace TaskTronic.Drive.Models.Folders
{
    using System;

    public class InputFolderServiceModel
    {
        public int CatalogId { get; set; }
        public int? ParentId { get; set; }
        public int? RootId { get; set; }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public int EmployeeId { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
