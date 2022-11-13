namespace TaskTronic.Drive.Models.Files
{
    using System;
    using System.Collections.Generic;

    public class OutputFileSearchServiceModel
    {
        public int FileId { get; set; }
        public int CatalogId { get; set; }
        public int FolderId { get; set; }
        public int BlobId { get; set; }
        public long FileSize { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string ContentType { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public string UpdaterUsername { get; set; }
        public List<string> SearchFolderNamesPath { get; set; }
        public int EmployeeId { get; set; }
    }
}
