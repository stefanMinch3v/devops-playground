namespace TaskTronic.Messages.Drive.Files
{
    public class FileUploadedMessage
    {
        public int FileId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }
}
