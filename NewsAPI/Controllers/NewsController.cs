using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAPI.Data;
using NewsAPI.Models.DTOs;
using NewsAPI.Models.Entities;

namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public NewsController(AppDbContext dbContext) 
        {
            this.dbContext = dbContext;
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
        public async Task<IActionResult> AddNews(AddNewsDto addNewsDto)
        {

            var newsId = Guid.NewGuid();

            var newsEntity = new News()
            {
                NewsId = Guid.NewGuid(),
                Title = addNewsDto.Title,
                ShortDescription = addNewsDto.ShortDescription,
                Content = addNewsDto.Content,
                Keywords = addNewsDto.Keywords,
                PublishedDate = addNewsDto.PublishedDate,
                CategoryId = addNewsDto.CategoryId,
                Images = addNewsDto.ImageUrls.Select(url => new NewsImage
                {
                    ImageId = Guid.NewGuid(),
                    ImagePath = url,
                    NewsId = newsId,
                    Title = "Default Image Title"
                }).ToList()
            };

            dbContext.News.Add(newsEntity);
            await dbContext.SaveChangesAsync();

            return Ok(newsEntity);

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
        public async Task<IActionResult> UpdateNews(Guid newsId, AddNewsDto addNewsDto)
        {
            var existingNews = await dbContext.News.Include(n => n.Images).FirstOrDefaultAsync(n => n.NewsId == newsId);

            if (existingNews == null)
            {
                return NotFound("Haber bulunamadı.");
            }

            // Güncellenmek istenen alanları ayarla
            existingNews.Title = addNewsDto.Title;
            existingNews.ShortDescription = addNewsDto.ShortDescription;
            existingNews.Content = addNewsDto.Content;
            existingNews.Keywords = addNewsDto.Keywords;
            existingNews.PublishedDate = addNewsDto.PublishedDate;
            existingNews.CategoryId = addNewsDto.CategoryId;

            // Resim ekleme
            foreach (var url in addNewsDto.ImageUrls)
            {
                existingNews.Images.Add(new NewsImage
                {
                    ImageId = Guid.NewGuid(),
                    ImagePath = url,
                    Title = "Default Image Title",
                    NewsId = newsId // mevcut haberin ID'si
                });
            }

            await dbContext.SaveChangesAsync();

            return Ok(existingNews);
        }





    }
}
