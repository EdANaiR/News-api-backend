namespace NewsAPI.Models.DTOs
{
    public class AddNewsDto
    {
        public string Title { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Keywords { get; set; } = new List<string>();
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public Guid CategoryId { get; set; }
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
