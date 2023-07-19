using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Utils.Constants.Settings;

namespace Google.BLL.Services
{
    public class FileService : IFileService
    {
        private readonly FileSettings fileConfiguration;

        public FileService(IOptions<FileSettings> fileConfiguration)
        {
            this.fileConfiguration = fileConfiguration.Value;
        }

        public Apis.Drive.v3.Data.File CreateFileMetadata(string path)
        {
            return new Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(path),
                MimeType = fileConfiguration.OriginalMimeType
            };
        }

        public FilesResource.CreateMediaUpload UploadFile(DriveService service, Apis.Drive.v3.Data.File fileMetadata, IFormFile originalFile)
        {
            using var stream = originalFile.OpenReadStream();

            var request = service.Files.Create(fileMetadata, stream, fileConfiguration.ContentType);
            request.Fields = "id";
            request.Upload();

            return request;
        }

        public async Task<Stream> CreateFileStreamAsync(DriveService service, string fileId)
        {
            var exportRequest = service.Files.Export(fileId, fileConfiguration.OutputMimeType);
            var streamResponse = await exportRequest.ExecuteAsStreamAsync();

            return streamResponse;
        }

        public string CreateNewFileName(string originalFileName)
        {
            return (Path.GetFileNameWithoutExtension(originalFileName) + ".pdf");
        }
    }
}
