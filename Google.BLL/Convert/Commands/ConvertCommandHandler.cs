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
        private readonly string outputDirectory = @"C:\Users\polin\Just_all\Work\Ilya\DocsConverting\Testing\Files\";

        public async Task<ConvertResult> Handle(ConvertCommand command, CancellationToken cancellationToken)
        {
            // Загрузка учетных данных службы
            GoogleCredential credential;
            using (var stream = new FileStream(serviceAccountKeyPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(DriveService.ScopeConstants.Drive);
            }

            // Инициализация службы Google Drive
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Converter"
            });

            // Загрузка исходного файла с локального компьютера
            var fileMetadata = new Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileNameWithoutExtension(command.OriginalFilePath) + ".pdf",
                MimeType = "application/pdf"
            };


            // Конвертация файла в формате .doc в .pdf
            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(command.OriginalFilePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                request.Fields = "id";
                await request.UploadAsync();
            }

            var convertedFileId = request.ResponseBody?.Id;

            if (convertedFileId != null)
            {
                var outputFileName = fileMetadata.Name;
                var outputFilePath = Path.Combine(outputDirectory, outputFileName);

                using (var stream = new FileStream(outputFilePath, FileMode.Create))
                {
                    service.Files.Export(convertedFileId, "application/pdf")
                        .Download(stream);
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
