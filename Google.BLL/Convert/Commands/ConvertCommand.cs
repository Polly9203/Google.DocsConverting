using Google.BLL.Convert.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Google.BLL.Convert.Commands
{
    public class ConvertCommand : IRequest<ConvertResult>
    {
        [JsonPropertyName("original_file")]
        public IFormFile OriginalFile { get; set; }
    }
}
