namespace NewsAPI.Models.DTOs
{
    public class AddNewsDto
    {
        public Guid NewsId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public List<string> Keywords { get; set; }
        public DateTime PublishedDate { get; set; }
        public List<string> ImageUrls { get; set; } // Image pathlerini içeren bir liste
        public Guid CategoryId { get; set; }
        
    }
}
