using System.ComponentModel.DataAnnotations;

namespace NewsAPI.Models.Entities
{
    public class NewsImage
    {
        
        public Guid ImageId { get; set; }
        public string ImagePath { get; set; }
        public string? Title { get; set; }  // Fotoğraf başlığı/açıklaması
          

        // Haber ile ilişki
        public Guid NewsId { get; set; }
        public News News { get; set; }

        


    }
}
