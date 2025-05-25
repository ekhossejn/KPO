using Microsoft.EntityFrameworkCore;
using Common.Models;

namespace FileAnalysisService.Data
{
    public class AnalysisDbContext : DbContext
    {
        public AnalysisDbContext(DbContextOptions<AnalysisDbContext> options) : base(options) { }
        public DbSet<TextAnalysisResult> AnalysisResults { get; set; }
    }
}
