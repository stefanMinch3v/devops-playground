namespace TaskTronic.Drive.Services.Files
{
    using Models.Files;
    using System.IO;
    using System.Threading.Tasks;

    public interface IFileService
    {
        Task ReadStreamFromFileAsync(int blobId, Stream stream);

        Task<bool> DeleteFileAsync(int employeeId, int catalogId, int folderId, int fileId);

        Task<bool> UploadFileAsync(InputFileServiceModel file);

        Task<OutputFileDownloadServiceModel> GetFileInfoForDownloadAsync(int catalogId, int employeeId, int folderId, int fileId);

        Task<bool> RenameFileAsync(int catalogId, int folderId, int fileId, int employeeId, string newFileName);

        Task<bool> CreateNewFileAsync(int catalogId, int employeeId, int folderId, string fileName, NewFileType fileType);
    }
}
