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
    public class CategoriesController : ControllerBase
    {

        private readonly AppDbContext _context;
        public CategoriesController (AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryDto addCategoryDto)
        {
            if (string.IsNullOrWhiteSpace(addCategoryDto.Name))
            {
                return BadRequest("Category name cannot be empty.");
            }

            var category = new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = addCategoryDto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }


        // Ek olarak tek bir kategoriyi alma metodu
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }


    }
}
