namespace NewsAPI.Models.DTOs
{
    public class NewsSummaryDto
    {
        public Guid NewsId { get; set; }
        public string? Title { get; set; }
        public string?ImagePath { get; set; }
        public string? ShortDescription { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
