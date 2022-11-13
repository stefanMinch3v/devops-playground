namespace TaskTronic.Drive.Data.Models
{
    using System;

    public class Blobsdata
    {
        public int BlobId { get; set; }
        public int EmployeeId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public bool FinishedUpload { get; set; }
        public DateTime Timestamp { get; set; }
        public byte[] Data { get; set; }
    }
}
