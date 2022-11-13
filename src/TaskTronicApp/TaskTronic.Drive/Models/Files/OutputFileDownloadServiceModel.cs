namespace TaskTronic.Drive.Models.Files
{
    public class OutputFileDownloadServiceModel
    {
        public int BlobId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
    }
}
