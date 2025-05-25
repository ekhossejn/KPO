using Microsoft.AspNetCore.Mvc;
using Common.Models;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TextScannerController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly string _fileStoringServiceUrl = "http://filestoring:5002";
        private readonly string _fileAnalysisServiceUrl = "http://fileanalysis:5003";

        public TextScannerController(HttpClient httpClient)
        {
            _client = httpClient;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<FileMetadata>> UploadFile(IFormFile file)
        {
            var data = new MultipartFormDataContent();
            var text = new StreamContent(file.OpenReadStream());
            data.Add(text, "file", file.FileName);

            var response = await _client.PostAsync($"{_fileStoringServiceUrl}/Files", data);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var metadata = await response.Content.ReadFromJsonAsync<FileMetadata>();
            return Ok(metadata);
        }

        [HttpGet("analyze/{fileId}")]
        public async Task<ActionResult<TextAnalysisResult>> AnalyzeFile(string fileId)
        {
            var response = await _client.PostAsync($"{_fileAnalysisServiceUrl}/Analysis/{fileId}", null);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var analysisResult = await response.Content.ReadFromJsonAsync<TextAnalysisResult>();
            return Ok(analysisResult);
        }


        [HttpGet("file/{fileId}")]
        public async Task<ActionResult<string>> GetFile(string fileId)
        {
            var response = await _client.GetAsync($"{_fileStoringServiceUrl}/Files/{fileId}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var text = await response.Content.ReadAsStringAsync();
            return Ok(text);
        }
    }
}

