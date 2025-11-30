using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Registers.Cart.Products;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Restaurant.Controllers.Registers.Cart
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly AppData _context;

        public ProductsController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedProductsResponseDto>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? categoryId = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(p =>
                    p.NameProduct.ToLower().Contains(search) ||
                    p.DescriptionProduct.ToLower().Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var total = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.NameProduct)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.NameProduct,
                    Price = p.Price,
                    Description = p.DescriptionProduct,
                    ImageUrl = p.ImageUrl,
                    IsActive = p.IsActive,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.NameCategory : null,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedProductsResponseDto
            {
                Products = products,
                Total = total,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            return Ok(new ProductResponseDto
            {
                Id = product.Id,
                Name = product.NameProduct,
                Price = product.Price,
                Description = product.DescriptionProduct,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.NameCategory,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "La categoría especificada no existe" });
            }

            if (await _context.Products.AnyAsync(p => p.NameProduct == dto.Name))
            {
                return BadRequest(new { message = "Ya existe un producto con ese nombre" });
            }

            var product = new Product
            {
                NameProduct = dto.Name,
                Price = dto.Price,
                DescriptionProduct = dto.Description ?? string.Empty,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new ProductResponseDto
            {
                Id = product.Id,
                Name = product.NameProduct,
                Price = product.Price,
                Description = product.DescriptionProduct,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.NameCategory,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
            {
                return BadRequest(new { message = "La categoría especificada no existe" });
            }

            if (await _context.Products.AnyAsync(p => p.NameProduct == dto.Name && p.Id != id))
            {
                return BadRequest(new { message = "Ya existe otro producto con ese nombre" });
            }

            product.NameProduct = dto.Name;
            product.Price = dto.Price;
            product.DescriptionProduct = dto.Description ?? string.Empty;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            return Ok(new ProductResponseDto
            {
                Id = product.Id,
                Name = product.NameProduct,
                Price = product.Price,
                Description = product.DescriptionProduct,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.NameCategory,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            var hasOrders = await _context.OrderDetails.AnyAsync(od => od.ProductId == id);

            if (hasOrders)
            {
                return BadRequest(new
                {
                    message = "No se puede eliminar el producto porque tiene órdenes asociadas. " +
                              "Considere desactivar el producto en su lugar."
                });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Producto eliminado exitosamente" });
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<ProductResponseDto>> ToggleProductStatus(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { message = "Producto no encontrado" });
            }

            product.IsActive = !product.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ProductResponseDto
            {
                Id = product.Id,
                Name = product.NameProduct,
                Price = product.Price,
                Description = product.DescriptionProduct,
                ImageUrl = product.ImageUrl,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.NameCategory,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            });
        }
    }
}
