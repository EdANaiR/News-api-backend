namespace NewsAPI.Models.DTOs
{
    public class NewsDetailDto
    {

        public Guid NewsId { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public List<string> Keywords { get; set; }
        public DateTime PublishedDate { get; set; }
        public List<string> ImagePaths { get; set; }
        public Guid CategoryId { get; set; }
    }
}
