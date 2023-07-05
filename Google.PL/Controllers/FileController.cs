using Google.BLL.Convert.Commands;
using Google.BLL.Convert.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Google.PL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : BaseController
    {
        public FileController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ConvertResult>> Convert([FromBody] ConvertCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
