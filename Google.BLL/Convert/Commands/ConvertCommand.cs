using Google.BLL.Convert.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Google.BLL.Convert.Commands
{
    public class ConvertCommand : IRequest<ConvertResult>
    {
        public IFormFile OriginalFile { get; set; }
    }
}
