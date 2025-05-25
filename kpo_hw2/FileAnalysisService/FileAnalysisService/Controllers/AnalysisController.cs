using Microsoft.AspNetCore.Mvc;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using FileAnalysisService.Data;
using System.Text;

namespace FileAnalysisService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly AnalysisDbContext _context;
        private readonly HttpClient _client;
        private readonly string _fileStoringSerrviceUrl;

        public AnalysisController(AnalysisDbContext context, HttpClient httpClient, IConfiguration config)
        {
            _context = context;
            _client = httpClient;
            _fileStoringSerrviceUrl = config["FileStorageUrl"] ?? "http://filestoring:5002";
        }

        [HttpPost("{fileId}")]
        public async Task<ActionResult<TextAnalysisResult>> AnalyzeFile(string fileId)
        {
            var gottenAnalysis = await _context.AnalysisResults.FirstOrDefaultAsync(a => a.FileId == fileId);
            if (gottenAnalysis != null)
            {
                return Ok(gottenAnalysis);
            }

            try
            {
                var response = await _client.GetAsync($"{_fileStoringSerrviceUrl}/Files/{fileId}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound("File not found");
                }

                var text = await response.Content.ReadAsStringAsync();
                var analysis = new TextAnalysisResult
                {
                    FileId = fileId,
                    ParagraphCount = CountParagraphs(text),
                    WordCount = CountWords(text),
                    CharacterCount = text.Length,
                };
                var otherFiles = await _context.AnalysisResults.ToListAsync();
                foreach (var otherFile in otherFiles)
                {
                    var otherText = await _client.GetStringAsync($"{_fileStoringSerrviceUrl}/Files/{otherFile.FileId}");
                    if (text == otherText)
                    {
                        analysis.IsPlagiarized = true;
                        analysis.MatchedFileId = otherFile.FileId;
                        break;
                    }
                }
                await _context.AnalysisResults.AddAsync(analysis);
                await _context.SaveChangesAsync();
                return Ok(analysis);
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, "File Storage Service is unavailable");
            }
        }

        private static int CountParagraphs(string text)
        {
            var normalized = text.Replace("\r\n", "\n").Replace("\r", "\n");
            return normalized.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private static int CountWords(string text)
        {
            return text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length;
        }

    }
}