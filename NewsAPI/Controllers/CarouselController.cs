using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Data;
using NewsAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using NewsAPI.Models.DTOs;
using Microsoft.AspNetCore.Hosting;
using System;


namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarouselController : ControllerBase
    {
        private readonly AppDbContext _context;
        

        public CarouselController(AppDbContext context)
        {
            _context = context;
           
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarouselDTO>>> GetCarouselNews()
        {
            var carouselNews = await _context.CarouselNews
                .OrderByDescending(n => n.PublishedDate)
                .Take(5)
                .Select(n => new CarouselDTO
                {
                    CarouselId = n.CarouselId,
                    Title = n.Title,
                    ShortDescription = n.ShortDescription,
                    Content = n.Content,
                    Keywords = n.Keywords,
                    PublishedDate = n.PublishedDate,
                    ImageUrls = n.Images.Select(img => img.ImagePath).ToList(),
                    CategoryId = n.CategoryId
                })
                .ToListAsync();

            return Ok(carouselNews);
        }

        // Yeni bir Carousel haberi ekle
        [HttpPost]
        public async Task<IActionResult> AddCarousel([FromBody] CarouselDTO addCarouselDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var carouselId = Guid.NewGuid();

            var carouselEntity = new Carousel
            {
                CarouselId = carouselId,
                Title = addCarouselDto.Title,
                ShortDescription = addCarouselDto.ShortDescription,
                Content = addCarouselDto.Content,
                Keywords = addCarouselDto.Keywords,
                PublishedDate = addCarouselDto.PublishedDate,
                CategoryId = addCarouselDto.CategoryId,
                Images = addCarouselDto.ImageUrls.Select(url => new NewsImage
                {
                    ImageId = Guid.NewGuid(),
                    ImagePath = url,
                    CarouselId = carouselId,
                }).ToList()
            };

            _context.CarouselNews.Add(carouselEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarouselNewsItem), new { id = carouselEntity.CarouselId }, addCarouselDto);
        }

        // Tek bir Carousel haberini getir
        [HttpGet("{id}")]
        public async Task<ActionResult<CarouselDTO>> GetCarouselNewsItem(Guid id)
        {
            var newsItem = await _context.CarouselNews
                .Include(n => n.Images)
                .FirstOrDefaultAsync(n => n.CarouselId == id);

            if (newsItem == null)
                return NotFound();

            var carouselDto = new CarouselDTO
            {
                CarouselId = newsItem.CarouselId,
                Title = newsItem.Title,
                ShortDescription = newsItem.ShortDescription,
                Content = newsItem.Content,
                Keywords = newsItem.Keywords,
                PublishedDate = newsItem.PublishedDate,
                ImageUrls = newsItem.Images.Select(img => img.ImagePath).ToList(),
                CategoryId = newsItem.CategoryId
            };

            return Ok(carouselDto);
        }
    }


}

