using Google.BLL.Convert.Models;
using MediatR;

namespace Google.BLL.Convert.Commands
{
    public class ConvertCommandHandler : IRequestHandler<ConvertCommand, ConvertResult>
    {
        public async Task<ConvertResult> Handle (ConvertCommand command, CancellationToken cancellationToken)
        {
            return new ConvertResult();
        }
    }
}
