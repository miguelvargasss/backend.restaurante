using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Registers.Feedback.Claims;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Restaurant.Controllers.Registers.Feedback
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClaimsController : ControllerBase
    {
        private readonly AppData _context;

        public ClaimsController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetClaims(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Claims.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(c =>
                    c.NameClaim.ToLower().Contains(search) ||
                    c.DetailClaim.ToLower().Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var total = await query.CountAsync();

            var claims = await query
                .OrderByDescending(c => c.ClaimDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ClaimResponseDto
                {
                    Id = c.Id,
                    Name = c.NameClaim,
                    Detail = c.DetailClaim,
                    ClaimDate = c.ClaimDate,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                claims,
                total,
                page,
                pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClaimResponseDto>> GetClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
            {
                return NotFound(new { message = "Reclamo no encontrado" });
            }

            return Ok(new ClaimResponseDto
            {
                Id = claim.Id,
                Name = claim.NameClaim,
                Detail = claim.DetailClaim,
                ClaimDate = claim.ClaimDate,
                IsActive = claim.IsActive,
                CreatedAt = claim.CreatedAt,
                UpdatedAt = claim.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<ClaimResponseDto>> CreateClaim([FromBody] CreateClaimDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var claim = new Claim
            {
                NameClaim = dto.Name,
                DetailClaim = dto.Detail,
                ClaimDate = dto.ClaimDate,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClaim), new { id = claim.Id }, new ClaimResponseDto
            {
                Id = claim.Id,
                Name = claim.NameClaim,
                Detail = claim.DetailClaim,
                ClaimDate = claim.ClaimDate,
                IsActive = claim.IsActive,
                CreatedAt = claim.CreatedAt,
                UpdatedAt = claim.UpdatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClaimResponseDto>> UpdateClaim(int id, [FromBody] UpdateClaimDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
            {
                return NotFound(new { message = "Reclamo no encontrado" });
            }

            claim.NameClaim = dto.Name;
            claim.DetailClaim = dto.Detail;
            claim.IsActive = dto.IsActive;
            claim.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ClaimResponseDto
            {
                Id = claim.Id,
                Name = claim.NameClaim,
                Detail = claim.DetailClaim,
                ClaimDate = claim.ClaimDate,
                IsActive = claim.IsActive,
                CreatedAt = claim.CreatedAt,
                UpdatedAt = claim.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClaim(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
            {
                return NotFound(new { message = "Reclamo no encontrado" });
            }

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reclamo eliminado exitosamente" });
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<ClaimResponseDto>> ToggleClaimStatus(int id)
        {
            var claim = await _context.Claims.FindAsync(id);

            if (claim == null)
            {
                return NotFound(new { message = "Reclamo no encontrado" });
            }

            claim.IsActive = !claim.IsActive;
            claim.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ClaimResponseDto
            {
                Id = claim.Id,
                Name = claim.NameClaim,
                Detail = claim.DetailClaim,
                ClaimDate = claim.ClaimDate,
                IsActive = claim.IsActive,
                CreatedAt = claim.CreatedAt,
                UpdatedAt = claim.UpdatedAt
            });
        }
    }
}
