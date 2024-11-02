using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Models.DTOs
{
    public class CarouselDTO
    {
        
        public Guid CarouselId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public List<string> Keywords { get; set; }
        public DateTime PublishedDate { get; set; }
        public List<string> ImageUrls { get; set; } // Image pathlerini içeren bir liste
        public Guid CategoryId { get; set; }
    }
}
