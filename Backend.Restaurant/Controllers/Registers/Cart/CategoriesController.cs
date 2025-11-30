using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Registers.Cart.Categories;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Restaurant.Controllers.Registers.Cart
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly AppData _context;

        public CategoriesController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedCategoriesResponseDto>> GetCategories(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Categories
                .Include(c => c.Products)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(c =>
                    c.NameCategory.ToLower().Contains(search) ||
                    (c.Description != null && c.Description.ToLower().Contains(search)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var total = await query.CountAsync();

            var categories = await query
                .OrderBy(c => c.NameCategory)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.NameCategory,
                    Description = c.Description,
                    IsActive = c.IsActive,
                    ProductCount = c.Products != null ? c.Products.Count : 0,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedCategoriesResponseDto
            {
                Categories = categories,
                Total = total,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("simple")]
        public async Task<IActionResult> GetSimpleCategories()
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.NameCategory)
                .Select(c => new
                {
                    id = c.Id,
                    name = c.NameCategory,
                    description = c.Description
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            return Ok(new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.NameCategory,
                Description = category.Description,
                IsActive = category.IsActive,
                ProductCount = category.Products?.Count ?? 0,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Categories.AnyAsync(c => c.NameCategory == dto.Name))
            {
                return BadRequest(new { message = "Ya existe una categoría con ese nombre" });
            }

            var category = new Category
            {
                NameCategory = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.NameCategory,
                Description = category.Description,
                IsActive = category.IsActive,
                ProductCount = 0,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            if (await _context.Categories.AnyAsync(c => c.NameCategory == dto.Name && c.Id != id))
            {
                return BadRequest(new { message = "Ya existe otra categoría con ese nombre" });
            }

            category.NameCategory = dto.Name;
            category.Description = dto.Description;
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.NameCategory,
                Description = category.Description,
                IsActive = category.IsActive,
                ProductCount = category.Products?.Count ?? 0,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            if (category.Products != null && category.Products.Any())
            {
                return BadRequest(new
                {
                    message = "No se puede eliminar la categoría porque tiene productos asociados. " +
                              "Considere desactivar la categoría en su lugar."
                });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Categoría eliminada exitosamente" });
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<CategoryResponseDto>> ToggleCategoryStatus(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = "Categoría no encontrada" });
            }

            category.IsActive = !category.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.NameCategory,
                Description = category.Description,
                IsActive = category.IsActive,
                ProductCount = category.Products?.Count ?? 0,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            });
        }
    }
}
