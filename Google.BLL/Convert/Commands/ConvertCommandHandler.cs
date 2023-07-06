using Google.BLL.Convert.Models;
using Google.BLL.Services;
using MediatR;

namespace Google.BLL.Convert.Commands
{
    public class ConvertCommandHandler : IRequestHandler<ConvertCommand, ConvertResult>
    {
        private readonly IGoogleService _googleService;
        private readonly IFileService _fileService;

        public ConvertCommandHandler(IGoogleService googleService, IFileService fileService)
        {
            _googleService = googleService;
            _fileService = fileService;
        }

        public async Task<ConvertResult> Handle(ConvertCommand command, CancellationToken cancellationToken)
        {
            var credential = _googleService.CreateCredentials();

            using var service = _googleService.CreateDriveService(credential);
            var fileMetadata = _fileService.CreateFileMetadata(command.OriginalFile.FileName);

            var request = _fileService.UploadFile(service, fileMetadata, command.OriginalFile);

            var exportedStream = await _fileService.ExportFileAsync(service, request.ResponseBody.Id);
            var outputFilePath = _fileService.SaveFile(exportedStream, command.OriginalFile.FileName);

            return new ConvertResult() { PdfFilePath = outputFilePath };
        }
    }
}
