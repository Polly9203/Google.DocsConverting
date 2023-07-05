using Google.BLL.Convert.Models;
using MediatR;
using System.Text.Json.Serialization;

namespace Google.BLL.Convert.Commands
{
    public class ConvertCommand : IRequest<ConvertResult>
    {
        [JsonPropertyName("original_path")]
        public string OriginalFilePath { get; set; }
    }
}
