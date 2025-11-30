using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Registers.Feedback.Suggestions;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Restaurant.Controllers.Registers.Feedback
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SuggestionsController : ControllerBase
    {
        private readonly AppData _context;

        public SuggestionsController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetSuggestions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? status = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Suggestions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(s =>
                    s.NameSuggestion.ToLower().Contains(search) ||
                    s.DetailsSuggestion.ToLower().Contains(search) ||
                    (s.ContactEmail != null && s.ContactEmail.ToLower().Contains(search)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(s => s.Status == status);
            }

            var total = await query.CountAsync();

            var suggestions = await query
                .OrderByDescending(s => s.SuggestionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SuggestionResponseDto
                {
                    Id = s.Id,
                    Name = s.NameSuggestion,
                    Details = s.DetailsSuggestion,
                    ContactEmail = s.ContactEmail,
                    Status = s.Status,
                    SuggestionDate = s.SuggestionDate,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                suggestions,
                total,
                page,
                pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SuggestionResponseDto>> GetSuggestion(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);

            if (suggestion == null)
            {
                return NotFound(new { message = "Sugerencia no encontrada" });
            }

            return Ok(new SuggestionResponseDto
            {
                Id = suggestion.Id,
                Name = suggestion.NameSuggestion,
                Details = suggestion.DetailsSuggestion,
                ContactEmail = suggestion.ContactEmail,
                Status = suggestion.Status,
                SuggestionDate = suggestion.SuggestionDate,
                IsActive = suggestion.IsActive,
                CreatedAt = suggestion.CreatedAt,
                UpdatedAt = suggestion.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<SuggestionResponseDto>> CreateSuggestion([FromBody] CreateSuggestionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var suggestion = new Suggestion
            {
                NameSuggestion = dto.Name,
                DetailsSuggestion = dto.Details,
                ContactEmail = dto.ContactEmail,
                SuggestionDate = dto.SuggestionDate,
                Status = "Pendiente",
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Suggestions.Add(suggestion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSuggestion), new { id = suggestion.Id }, new SuggestionResponseDto
            {
                Id = suggestion.Id,
                Name = suggestion.NameSuggestion,
                Details = suggestion.DetailsSuggestion,
                ContactEmail = suggestion.ContactEmail,
                Status = suggestion.Status,
                SuggestionDate = suggestion.SuggestionDate,
                IsActive = suggestion.IsActive,
                CreatedAt = suggestion.CreatedAt,
                UpdatedAt = suggestion.UpdatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SuggestionResponseDto>> UpdateSuggestion(int id, [FromBody] UpdateSuggestionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var suggestion = await _context.Suggestions.FindAsync(id);

            if (suggestion == null)
            {
                return NotFound(new { message = "Sugerencia no encontrada" });
            }

            suggestion.NameSuggestion = dto.Name;
            suggestion.DetailsSuggestion = dto.Details;
            suggestion.ContactEmail = dto.ContactEmail;
            suggestion.Status = dto.Status;
            suggestion.IsActive = dto.IsActive;
            suggestion.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new SuggestionResponseDto
            {
                Id = suggestion.Id,
                Name = suggestion.NameSuggestion,
                Details = suggestion.DetailsSuggestion,
                ContactEmail = suggestion.ContactEmail,
                Status = suggestion.Status,
                SuggestionDate = suggestion.SuggestionDate,
                IsActive = suggestion.IsActive,
                CreatedAt = suggestion.CreatedAt,
                UpdatedAt = suggestion.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuggestion(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);

            if (suggestion == null)
            {
                return NotFound(new { message = "Sugerencia no encontrada" });
            }

            _context.Suggestions.Remove(suggestion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sugerencia eliminada exitosamente" });
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<SuggestionResponseDto>> ToggleSuggestionStatus(int id)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);

            if (suggestion == null)
            {
                return NotFound(new { message = "Sugerencia no encontrada" });
            }

            suggestion.IsActive = !suggestion.IsActive;
            suggestion.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new SuggestionResponseDto
            {
                Id = suggestion.Id,
                Name = suggestion.NameSuggestion,
                Details = suggestion.DetailsSuggestion,
                ContactEmail = suggestion.ContactEmail,
                Status = suggestion.Status,
                SuggestionDate = suggestion.SuggestionDate,
                IsActive = suggestion.IsActive,
                CreatedAt = suggestion.CreatedAt,
                UpdatedAt = suggestion.UpdatedAt
            });
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<SuggestionResponseDto>> UpdateSuggestionStatus(int id, [FromBody] string status)
        {
            var suggestion = await _context.Suggestions.FindAsync(id);

            if (suggestion == null)
            {
                return NotFound(new { message = "Sugerencia no encontrada" });
            }

            suggestion.Status = status;
            suggestion.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new SuggestionResponseDto
            {
                Id = suggestion.Id,
                Name = suggestion.NameSuggestion,
                Details = suggestion.DetailsSuggestion,
                ContactEmail = suggestion.ContactEmail,
                Status = suggestion.Status,
                SuggestionDate = suggestion.SuggestionDate,
                IsActive = suggestion.IsActive,
                CreatedAt = suggestion.CreatedAt,
                UpdatedAt = suggestion.UpdatedAt
            });
        }
    }
}
