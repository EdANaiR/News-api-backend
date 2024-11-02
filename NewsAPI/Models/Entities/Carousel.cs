using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Models.Entities
{
    public class Carousel
    {
        [Key]
        public Guid CarouselId { get; set; }

        public required string Title { get; set; }

        public required string ShortDescription { get; set; }

        public required string Content { get; set; }

        public List<string> Keywords { get; set; }

        public DateTime PublishedDate { get; set; }

        public ICollection<CarouselImage> Images { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
