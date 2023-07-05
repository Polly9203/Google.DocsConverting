using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Google.PL.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IMediator Mediator { get; set; }

        public BaseController(IMediator mediator)
        {
            this.Mediator = mediator;
        }
    }
}
