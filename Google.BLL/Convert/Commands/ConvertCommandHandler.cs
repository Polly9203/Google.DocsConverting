using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.BLL.Convert.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Utils.Constants.Configuration;

namespace Google.BLL.Convert.Commands
{
    public class ConvertCommandHandler : IRequestHandler<ConvertCommand, ConvertResult>
    {
        private readonly GoogleConfiguration googleConfiguration;
        private readonly FileConfiguration fileConfiguration;

        public ConvertCommandHandler(IOptions<GoogleConfiguration> googleConfiguration, IOptions<FileConfiguration> fileConfiguration)
        {
            this.googleConfiguration = googleConfiguration.Value;
            this.fileConfiguration = fileConfiguration.Value;
        }

        public async Task<ConvertResult> Handle(ConvertCommand command, CancellationToken cancellationToken)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(googleConfiguration.ServiceAccountKeyPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(DriveService.ScopeConstants.Drive);
            }

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = googleConfiguration.ApplicationName
            });

            var fileMetadata = new Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(command.OriginalFile.FileName),
                MimeType = fileConfiguration.OriginalMimeType
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = command.OriginalFile.OpenReadStream())
            {
                request = service.Files.Create(fileMetadata, stream, fileConfiguration.ContentType);
                request.Fields = "id";
                request.Upload();
            }

            var convertedFileId = request.ResponseBody?.Id;

            if (convertedFileId != null)
            {
                var exportRequest = service.Files.Export(convertedFileId, fileConfiguration.OutputMimeType);
                var streamResponse = exportRequest.ExecuteAsStream();

                var outputFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var outputFilePath = Path.Combine(outputFolderPath, Path.GetFileNameWithoutExtension(command.OriginalFile.FileName) + ".pdf");

                using (var outputStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    streamResponse.CopyTo(outputStream);
                }

                return new ConvertResult() { PdfFilePath = outputFilePath };
            }
            else
            {
                return new ConvertResult() { PdfFilePath = "Failed" };
            }
        }
    }
}
