using Microsoft.AspNetCore.Mvc;
using Common.Models;
using System.Security.Cryptography;
using System.Text;
using FileStoringService.Data;
using Microsoft.EntityFrameworkCore;

namespace FileStoringService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FileDbContext _context;
        private readonly string _storagePath;

        public FilesController(FileDbContext context, IConfiguration config)
        {
            _context = context;
            _storagePath = config["StoragePath"] ?? Path.Combine(AppContext.BaseDirectory, "FileStorage");
            Directory.CreateDirectory(_storagePath);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || Path.GetExtension(file.FileName).ToLower() != ".txt")
            {
                return BadRequest("Можно загружать только .txt файлы.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var text = stream.ToArray();
            var hash = Convert.ToBase64String(SHA256.HashData(text));
            var gottenMetadata = await _context.Files.FirstOrDefaultAsync(f => f.Hash == hash);
            if (gottenMetadata != null)
            {
                return Ok(gottenMetadata);
            }

            var id = Guid.NewGuid().ToString();
            var location = $"{id}-{file.FileName}";
            var metadata = new FileMetadata
            {
                Id = id,
                Name = file.FileName,
                Hash = hash,
                Location = location
            };
            await _context.Files.AddAsync(metadata);
            await _context.SaveChangesAsync();

            var path = Path.Combine(_storagePath, location);
            await System.IO.File.WriteAllTextAsync(path, Encoding.UTF8.GetString(text));
            return Ok(metadata);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetFile(string id)
        {
            var metadata = await _context.Files.FirstOrDefaultAsync(f => f.Id == id);
            if (metadata == null)
            {
                return NotFound();
            }
            var path = Path.Combine(_storagePath, metadata.Location);
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var text = await System.IO.File.ReadAllTextAsync(path);
            return Ok(text);
        }
    }
}
