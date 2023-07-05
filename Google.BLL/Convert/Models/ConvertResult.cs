using System.Text.Json.Serialization;

namespace Google.BLL.Convert.Models
{
    public class ConvertResult
    {
        [JsonPropertyName("pdf_file_path")]
        public string PdfFilePath { get; set; }
    }
}
