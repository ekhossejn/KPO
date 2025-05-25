namespace Common.Models
{
    
    public class TextAnalysisResult
    {
        public int Id { get; set; }
        public string FileId { get; set; } = string.Empty;
        public int ParagraphCount { get; set; }
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }
        public bool IsPlagiarized { get; set; }
        public string? MatchedFileId { get; set; }
    }
}
