using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsAPI.Data;
using NewsAPI.Models.DTOs;
using NewsAPI.Models.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger<NewsController> _logger;
        private readonly IWebHostEnvironment _environment;

        public NewsController(AppDbContext dbContext, ILogger<NewsController> logger, IWebHostEnvironment environment)
        {
            this.dbContext = dbContext;
            _logger = logger;
            _environment = environment;
        }

    [HttpGet]
        public async Task<IActionResult> GetAllNews()
        {
            try
            {
                var allNews = await dbContext.News.Include(n => n.Category).ToListAsync();
                return Ok(allNews);
            }
            catch (Exception ex)
            {
                // Hata durumunda uygun bir cevap dönebilirsiniz
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNews([FromForm] AddNewsDto addNewsDto)
        {
            try
            {
                var newsId = Guid.NewGuid();

                var newsEntity = new News()
                {
                    NewsId = newsId,
                    Title = addNewsDto.Title,
                    ShortDescription = addNewsDto.ShortDescription,
                    Content = addNewsDto.Content,
                    Keywords = addNewsDto.Keywords,
                    PublishedDate = addNewsDto.PublishedDate,
                    CategoryId = addNewsDto.CategoryId,
                    Images = new List<NewsImage>()
                };

                if (addNewsDto.Images != null && addNewsDto.Images.Any())
                {
                    foreach (var image in addNewsDto.Images)
                    {
                        var imagePath = await SaveImage(image);
                        if (imagePath != null)
                        {
                            newsEntity.Images.Add(new NewsImage
                            {
                                ImageId = Guid.NewGuid(),
                                ImagePath = imagePath,
                                NewsId = newsId,
                                Title = "Default Image Title"
                            });
                        }
                        else
                        {
                            _logger.LogWarning($"Failed to save image for news: {newsId}");
                        }
                    }
                }

                dbContext.News.Add(newsEntity);
                await dbContext.SaveChangesAsync();

                return Ok(newsEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding news");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(Guid id)
        {
            try
            {
                var newsEntity = await dbContext.News.Include(n => n.Images).FirstOrDefaultAsync(n => n.NewsId == id);

                if (newsEntity == null)
                    return NotFound($"News with ID {id} not found.");

                dbContext.News.Remove(newsEntity);
                await dbContext.SaveChangesAsync();

                return Ok($"News with ID {id} has been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{newsId}")]
        public async Task<IActionResult> UpdateNews(Guid newsId, [FromForm] AddNewsDto addNewsDto)
        {
            var existingNews = await dbContext.News.Include(n => n.Images).FirstOrDefaultAsync(n => n.NewsId == newsId);

            if (existingNews == null)
            {
                return NotFound("Haber bulunamadı.");
            }

            existingNews.Title = addNewsDto.Title;
            existingNews.ShortDescription = addNewsDto.ShortDescription;
            existingNews.Content = addNewsDto.Content;
            existingNews.Keywords = addNewsDto.Keywords;
            existingNews.PublishedDate = addNewsDto.PublishedDate;
            existingNews.CategoryId = addNewsDto.CategoryId;

            if (addNewsDto.Images != null && addNewsDto.Images.Any())
            {
                foreach (var image in addNewsDto.Images)
                {
                    var imagePath = await SaveImage(image);
                    existingNews.Images.Add(new NewsImage
                    {
                        ImageId = Guid.NewGuid(),
                        ImagePath = imagePath,
                        Title = "Default Image Title",
                        NewsId = newsId
                    });
                }
            }

            await dbContext.SaveChangesAsync();

            return Ok(existingNews);
        }


        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetNewsByCategory(Guid categoryId)
        {
            try
            {
                var newsSummaries = await dbContext.News
                    .Where(n => n.CategoryId == categoryId)
                    .Select(n => new NewsSummaryDto
                    {
                        NewsId = n.NewsId,
                        Title = n.Title,
                        ImagePath = n.Images.FirstOrDefault().ImagePath, // İlk görseli al
                        ShortDescription = n.ShortDescription,          // Kısa açıklamayı ekle
                        PublishedDate = n.PublishedDate                // Yayın tarihini ekle
                    })
                    .Take(6) // 6 haberle sınırlama
                    .ToListAsync();

                if (!newsSummaries.Any())
                {
                    return NotFound($"No news found for the specified category ID: {categoryId}");
                }

                return Ok(newsSummaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        private async Task<string> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                _logger.LogWarning("Attempted to save a null or empty image file.");
                return null;
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            _logger.LogInformation($"Uploads folder path: {uploadsFolder}");

            if (!Directory.Exists(uploadsFolder))
            {
                _logger.LogInformation($"Creating uploads folder: {uploadsFolder}");
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            _logger.LogInformation($"Saving image to: {filePath}");

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            _logger.LogInformation($"Image saved successfully: {uniqueFileName}");

            return "/uploads/" + uniqueFileName;
        }


        [HttpGet("carousel")]
        public async Task<IActionResult> GetCarouselItems()
        {
            try
            {
                var carouselItems = await dbContext.News
                    .Where(n => n.PublishedDate <= DateTime.Now)
                    .Select(n => new
                    {
                        n.NewsId,
                        n.Title,
                        ImageUrl = n.Images.FirstOrDefault().ImagePath
                    })
                    .Where(item => item.ImageUrl != null)
                    .Take(10)
                    .ToListAsync();

                return Ok(carouselItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching carousel items");
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsDetail(Guid id)
        {
            try
            {
                // News entity'sini ve ilişkili verileri almak için sorguyu yapılandırıyoruz
                var newsEntity = await dbContext.News
                    .Include(n => n.Images)
                    .FirstOrDefaultAsync(n => n.NewsId == id);

                // İlgili News kaydı bulunamazsa NotFound döndürüyoruz
                if (newsEntity == null)
                {
                    return NotFound($"News with ID {id} not found.");
                }

                // News entity'sini NewsDetailDto'ya map ediyoruz
                var newsDetailDto = new NewsDetailDto
                {
                    NewsId = newsEntity.NewsId,
                    Title = newsEntity.Title,
                    ShortDescription = newsEntity.ShortDescription,
                    Content = newsEntity.Content,
                    Keywords = newsEntity.Keywords.ToList(),
                    PublishedDate = newsEntity.PublishedDate,
                    ImagePaths = newsEntity.Images.Select(img => img.ImagePath).ToList(),
                    CategoryId = newsEntity.CategoryId  // CategoryId'yi ekliyoruz
                };

                // Sonucu 200 OK yanıtı ile döndürüyoruz
                return Ok(newsDetailDto);
            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajı ve durum kodu döndürüyoruz
                _logger.LogError(ex, $"Error occurred while fetching news detail for ID {id}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("astroloji-news")]
        public IActionResult GetAstrolojiNews()
        {
            var astrolojiNews = dbContext.News 
                .Include(n => n.Category)
                .Where(n => n.Category.Name == "Astroloji")
                .Select(n => new 
                {
                    NewsId = n.NewsId,
                    Title = n.Title,
                    PublishedDate = n.PublishedDate
                })
                .ToList();

            return Ok(astrolojiNews);
        }


        [HttpGet("astroloji-news/{id}")]
        public async Task<IActionResult> GetAstrolojiNewsDetail(Guid id)
        {
            try
            {
                // Veritabanından News entity'sini ilişkili verilerle birlikte alıyoruz
                var newsEntity = await dbContext.News
                    .Include(n => n.Images) // Görselleri dahil ediyoruz
                    .Include(n => n.Category) // Kategori ilişkisini dahil ediyoruz
                    .FirstOrDefaultAsync(n => n.NewsId == id);

                // Eğer News bulunamazsa 404 dönüyoruz
                if (newsEntity == null)
                {
                    return NotFound(new { Message = $"News with ID {id} not found." });
                }

                // News entity'sini DTO'ya dönüştürüyoruz
                var newsDetailDto = new NewsDetailDto
                {
                    NewsId = newsEntity.NewsId,
                    Title = newsEntity.Title,
                    ShortDescription = newsEntity.ShortDescription ?? "Kısa açıklama mevcut değil", // Varsayılan değer
                    Content = newsEntity.Content,
                    Keywords = newsEntity.Keywords?.ToList() ?? new List<string>(), // Null kontrolü
                    PublishedDate = newsEntity.PublishedDate,
                    ImagePaths = newsEntity.Images?.Select(img => img.ImagePath).ToList() ?? new List<string>(),
                    CategoryId = newsEntity.CategoryId // Kategori ID'sini ekliyoruz
                };

                return Ok(newsDetailDto);
            }
            catch (Exception ex)
            {
                // Hata durumunda 500 döndürerek hata mesajını logluyoruz
                _logger.LogError(ex, $"Error occurred while fetching news detail for ID {id}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("breaking-news")]
        public IActionResult GetBreakingNews()
        {
            var breakingNews = dbContext.News
                .Include(n => n.Category)
                .Where(n => n.Category.Name == "Son Dakika")
                .Select(n => new
                {
                    NewsId = n.NewsId,
                    Title = n.Title,
                    
                })
                .Take(5)
                .ToList();

            return Ok(breakingNews);
        }

        [HttpGet("breaking-news/{id}")]
        public async Task<IActionResult> GetBreakingNewsDetail(Guid id)
        {
            try
            {
                // Veritabanından News entity'sini ilişkili verilerle birlikte alıyoruz
                var newsEntity = await dbContext.News
                    .Include(n => n.Images) // Görselleri dahil ediyoruz
                    .Include(n => n.Category) // Kategori ilişkisini dahil ediyoruz
                    .FirstOrDefaultAsync(n => n.NewsId == id);

                // Eğer News bulunamazsa 404 dönüyoruz
                if (newsEntity == null)
                {
                    return NotFound(new { Message = $"News with ID {id} not found." });
                }

                // News entity'sini DTO'ya dönüştürüyoruz
                var newsDetailDto = new NewsDetailDto
                {
                    NewsId = newsEntity.NewsId,
                    Title = newsEntity.Title,
                    ShortDescription = newsEntity.ShortDescription ?? "Kısa açıklama mevcut değil", // Varsayılan değer
                    Content = newsEntity.Content,
                    Keywords = newsEntity.Keywords?.ToList() ?? new List<string>(), // Null kontrolü
                    PublishedDate = newsEntity.PublishedDate,
                    ImagePaths = newsEntity.Images?.Select(img => img.ImagePath).ToList() ?? new List<string>(),
                    CategoryId = newsEntity.CategoryId // Kategori ID'sini ekliyoruz
                };

                return Ok(newsDetailDto);
            }
            catch (Exception ex)
            {
                // Hata durumunda 500 döndürerek hata mesajını logluyoruz
                _logger.LogError(ex, $"Error occurred while fetching news detail for ID {id}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }

}