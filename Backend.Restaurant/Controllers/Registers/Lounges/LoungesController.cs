using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Registers.Lounges;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Restaurant.Controllers.Registers.Lounges
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoungesController : ControllerBase
    {
        private readonly AppData _context;

        public LoungesController(AppData context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtener todos los salones con paginación y búsqueda
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedLoungesResponseDto>> GetLounges(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Lounges
                .Include(l => l.Tables)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(l => l.NameLounge.ToLower().Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(l => l.IsActive == isActive.Value);
            }

            var total = await query.CountAsync();

            var lounges = await query
                .OrderBy(l => l.NameLounge)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LoungeResponseDto
                {
                    Id = l.Id,
                    Name = l.NameLounge,
                    IsActive = l.IsActive,
                    TablesCount = l.Tables != null ? l.Tables.Count : 0,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedLoungesResponseDto
            {
                Lounges = lounges,
                Total = total,
                Page = page,
                PageSize = pageSize
            });
        }

        /// <summary>
        /// Obtener lista simple de salones activos (para dropdowns)
        /// </summary>
        [HttpGet("simple")]
        public async Task<IActionResult> GetSimpleLounges()
        {
            var lounges = await _context.Lounges
                .Where(l => l.IsActive)
                .OrderBy(l => l.NameLounge)
                .Select(l => new
                {
                    id = l.Id,
                    name = l.NameLounge
                })
                .ToListAsync();

            return Ok(lounges);
        }

        /// <summary>
        /// Obtener un salón por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LoungeResponseDto>> GetLounge(int id)
        {
            var lounge = await _context.Lounges
                .Include(l => l.Tables)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lounge == null)
            {
                return NotFound(new { message = "Salón no encontrado" });
            }

            return Ok(new LoungeResponseDto
            {
                Id = lounge.Id,
                Name = lounge.NameLounge,
                IsActive = lounge.IsActive,
                TablesCount = lounge.Tables?.Count ?? 0,
                CreatedAt = lounge.CreatedAt,
                UpdatedAt = lounge.UpdatedAt
            });
        }

        /// <summary>
        /// Crear un nuevo salón
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LoungeResponseDto>> CreateLounge([FromBody] CreateLoungeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que no exista un salón con el mismo nombre
            if (await _context.Lounges.AnyAsync(l => l.NameLounge == dto.Name))
            {
                return BadRequest(new { message = "Ya existe un salón con ese nombre" });
            }

            var lounge = new Lounge
            {
                NameLounge = dto.Name,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Lounges.Add(lounge);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLounge), new { id = lounge.Id }, new LoungeResponseDto
            {
                Id = lounge.Id,
                Name = lounge.NameLounge,
                IsActive = lounge.IsActive,
                TablesCount = 0,
                CreatedAt = lounge.CreatedAt,
                UpdatedAt = lounge.UpdatedAt
            });
        }

        /// <summary>
        /// Actualizar un salón existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<LoungeResponseDto>> UpdateLounge(int id, [FromBody] UpdateLoungeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var lounge = await _context.Lounges
                .Include(l => l.Tables)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lounge == null)
            {
                return NotFound(new { message = "Salón no encontrado" });
            }

            // Validar que no exista otro salón con el mismo nombre
            if (await _context.Lounges.AnyAsync(l => l.NameLounge == dto.Name && l.Id != id))
            {
                return BadRequest(new { message = "Ya existe otro salón con ese nombre" });
            }

            lounge.NameLounge = dto.Name;
            lounge.IsActive = dto.IsActive;
            lounge.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new LoungeResponseDto
            {
                Id = lounge.Id,
                Name = lounge.NameLounge,
                IsActive = lounge.IsActive,
                TablesCount = lounge.Tables?.Count ?? 0,
                CreatedAt = lounge.CreatedAt,
                UpdatedAt = lounge.UpdatedAt
            });
        }

        /// <summary>
        /// Eliminar un salón
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLounge(int id)
        {
            var lounge = await _context.Lounges
                .Include(l => l.Tables)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lounge == null)
            {
                return NotFound(new { message = "Salón no encontrado" });
            }

            // Verificar si tiene mesas asociadas
            if (lounge.Tables != null && lounge.Tables.Any())
            {
                return BadRequest(new
                {
                    message = $"No se puede eliminar el salón porque tiene {lounge.Tables.Count} mesa(s) asociada(s). " +
                              "Considere desactivar el salón en su lugar o reasignar las mesas primero."
                });
            }

            _context.Lounges.Remove(lounge);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Salón eliminado exitosamente" });
        }

        /// <summary>
        /// Activar/Desactivar un salón
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<LoungeResponseDto>> ToggleLoungeStatus(int id)
        {
            var lounge = await _context.Lounges
                .Include(l => l.Tables)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lounge == null)
            {
                return NotFound(new { message = "Salón no encontrado" });
            }

            lounge.IsActive = !lounge.IsActive;
            lounge.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new LoungeResponseDto
            {
                Id = lounge.Id,
                Name = lounge.NameLounge,
                IsActive = lounge.IsActive,
                TablesCount = lounge.Tables?.Count ?? 0,
                CreatedAt = lounge.CreatedAt,
                UpdatedAt = lounge.UpdatedAt
            });
        }

        /// <summary>
        /// Obtener todas las mesas de un salón específico
        /// </summary>
        [HttpGet("{id}/tables")]
        public async Task<IActionResult> GetLoungeWithTables(int id)
        {
            var lounge = await _context.Lounges
                .Include(l => l.Tables)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lounge == null)
            {
                return NotFound(new { message = "Salón no encontrado" });
            }

            var tablesList = lounge.Tables?.Select(t => new
            {
                id = t.Id,
                name = t.NameTable,
                environment = t.Environment,
                capacity = t.Capacity,
                isActive = t.IsActive
            }).ToList();

            return Ok(new
            {
                id = lounge.Id,
                name = lounge.NameLounge,
                isActive = lounge.IsActive,
                tablesCount = tablesList?.Count ?? 0,
                tables = tablesList
            });
        }
    }
}
