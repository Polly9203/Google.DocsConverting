using Google.BLL.Convert.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Utils.Constants.Settings;

namespace Google.PL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : BaseController
    {
        private readonly FileSettings fileConfiguration;

        public FileController(IMediator mediator, IOptions<FileSettings> fileConfiguration) : base(mediator)
        {
            this.fileConfiguration = fileConfiguration.Value;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Convert(IFormFile original_file)
        {
            var command = new ConvertCommand() { OriginalFile = original_file };
            var result = await Mediator.Send(command);

            return File(result.PdfFileStream, fileConfiguration.OutputMimeType, result.NewFileName);
        }
    }
}
