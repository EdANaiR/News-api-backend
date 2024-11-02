using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Models.Entities
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; }
        public string Name { get; set; }

        // Bir kategoriye ait birden çok haber
        public ICollection<News> NewsArticles { get; set; }
        public ICollection <Carousel> Carousels { get; set; }
    }
}
