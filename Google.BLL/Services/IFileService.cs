using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Http;

namespace Google.BLL.Services
{
    public interface IFileService
    {
        Apis.Drive.v3.Data.File CreateFileMetadata(string path);
        FilesResource.CreateMediaUpload UploadFile(DriveService service, Apis.Drive.v3.Data.File fileMetadata, IFormFile originalFile);
        Task<Stream> CreateFileStreamAsync(DriveService service, string fileId);
        string CreateNewFileName(string originalFileName);
    }
}
