namespace NewsAPI.Models.DTOs
{
    public class AddNewsDto
    {
        
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public List<string> Keywords { get; set; }
        public DateTime PublishedDate { get; set; }       
        public Guid CategoryId { get; set; }
        public List<IFormFile> Images { get; set; }


    }
}
