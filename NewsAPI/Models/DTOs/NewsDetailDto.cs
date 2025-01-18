namespace NewsAPI.Models.DTOs
{
    public class NewsDetailDto
    {
        public Guid NewsId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Keywords { get; set; } = new List<string>();
        public DateTime PublishedDate { get; set; }
        public List<string> ImagePaths { get; set; } = new List<string>();
        public Guid CategoryId { get; set; }
    }
}
