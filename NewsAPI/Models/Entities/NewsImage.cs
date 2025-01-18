using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Models.Entities
{
    public class NewsImage
    {
        
        public Guid ImageId { get; set; }
        [Required]
        public string? ImagePath { get; set; }
        [MaxLength(300)]
        public string? Title { get; set; }  // Fotoğraf başlığı/açıklaması
          

        // Haber ile ilişki
        public Guid NewsId { get; set; }
        public News? News { get; set; }

        


    }
}
