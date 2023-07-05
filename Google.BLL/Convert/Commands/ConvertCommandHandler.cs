using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.BLL.Convert.Models;
using MediatR;

namespace Google.BLL.Convert.Commands
{
    public class ConvertCommandHandler : IRequestHandler<ConvertCommand, ConvertResult>
    {
        private static string[] Scopes = { DriveService.Scope.Drive };
        private static string ApplicationName = "Converter";

        public async Task<ConvertResult> Handle(ConvertCommand command, CancellationToken cancellationToken)
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "Web client 1",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Upload the doc file to Google Drive
            var fileMetadata = new Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(command.OriginalFilePath),
                MimeType = "application/vnd.google-apps.document"
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(command.OriginalFilePath, FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                request.Fields = "id";
                request.Upload();
            }

            var uploadedFile = request.ResponseBody;

            // Convert the doc file to pdf
            var exportRequest = service.Files.Export(uploadedFile.Id, "application/pdf");
            var streamResponse = exportRequest.ExecuteAsStream();

            var pdfFilePath = Path.ChangeExtension(command.OriginalFilePath, ".pdf");
            using (var outputStream = new FileStream(pdfFilePath, FileMode.Create))
            {
                streamResponse.CopyTo(outputStream);
            }

            return new ConvertResult() {PdfFilePath = pdfFilePath};
        }
    }
}
