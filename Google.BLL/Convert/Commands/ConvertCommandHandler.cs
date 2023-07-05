using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.BLL.Convert.Models;
using MediatR;

namespace Google.BLL.Convert.Commands
{
    public class ConvertCommandHandler : IRequestHandler<ConvertCommand, ConvertResult>
    {
        private readonly string serviceAccountKeyPath = @"C:\Users\polin\Just_all\Work\Ilya\DocsConverting\SecurityKey.json";

        public async Task<ConvertResult> Handle(ConvertCommand command, CancellationToken cancellationToken)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(serviceAccountKeyPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(DriveService.ScopeConstants.Drive);
            }

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Converter"
            });

            var fileMetadata = new Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(command.OriginalFile.FileName),
                MimeType = "application/vnd.google-apps.document"
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = command.OriginalFile.OpenReadStream())
            {
                request = service.Files.Create(fileMetadata, stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                request.Fields = "id";
                request.Upload();
            }

            var convertedFileId = request.ResponseBody?.Id;

            if (convertedFileId != null)
            {
                var exportRequest = service.Files.Export(convertedFileId, "application/pdf");
                var streamResponse = exportRequest.ExecuteAsStream();

                var PdfFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var pdfFilePath = Path.Combine(PdfFolderPath, Path.GetFileNameWithoutExtension(command.OriginalFile.FileName) + ".pdf");

                using (var outputStream = new FileStream(pdfFilePath, FileMode.Create))
                {
                    streamResponse.CopyTo(outputStream);
                }

                return new ConvertResult() { PdfFilePath = pdfFilePath };
            }
            else
            {
                return new ConvertResult() { PdfFilePath = "Failed" };
            }
        }
    }
}
