using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.SmallBox;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Restaurant.Controllers.SmallBox
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SmallBoxController : ControllerBase
    {
        private readonly AppData _context;

        public SmallBoxController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedSmallBoxResponseDto>> GetSmallBoxes(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isClosed = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.SmallBoxes
                .Include(sb => sb.User)
                .Include(sb => sb.CashMovements)
                .AsQueryable();

            if (isClosed.HasValue)
            {
                query = query.Where(sb => sb.IsClosed == isClosed.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(sb => sb.OpeningDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(sb => sb.OpeningDate <= endDate.Value);
            }

            var total = await query.CountAsync();

            var smallBoxes = await query
                .OrderByDescending(sb => sb.OpeningDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(sb => new SmallBoxResponseDto
                {
                    Id = sb.Id,
                    InitialAmount = sb.InitialAmount,
                    FinalAmount = sb.FinalAmount,
                    OpeningDate = sb.OpeningDate,
                    ClosingDate = sb.ClosingDate,
                    AdditionalNote = sb.AdditionalNote,
                    IsClosed = sb.IsClosed,
                    UserId = sb.UserId,
                    UserName = sb.User != null ? $"{sb.User.NameUser} {sb.User.LastNameUser}" : null,
                    TotalIncome = sb.CashMovements!.Where(cm => cm.MovementType == "Ingreso").Sum(cm => cm.Amount),
                    TotalExpense = sb.CashMovements!.Where(cm => cm.MovementType == "Egreso").Sum(cm => cm.Amount),
                    CurrentBalance = sb.InitialAmount +
                        sb.CashMovements!.Where(cm => cm.MovementType == "Ingreso").Sum(cm => cm.Amount) -
                        sb.CashMovements!.Where(cm => cm.MovementType == "Egreso").Sum(cm => cm.Amount),
                    CashMovements = sb.CashMovements!.Select(cm => new CashMovementDto
                    {
                        Id = cm.Id,
                        MovementType = cm.MovementType,
                        Amount = cm.Amount,
                        Concept = cm.Concept,
                        MovementDate = cm.MovementDate
                    }).ToList(),
                    CreatedAt = sb.CreatedAt,
                    UpdatedAt = sb.UpdatedAt
                })
                .ToListAsync();

            var response = new PaginatedSmallBoxResponseDto
            {
                SmallBoxes = smallBoxes,
                Total = total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SmallBoxResponseDto>> GetSmallBox(int id)
        {
            var smallBox = await _context.SmallBoxes
                .Include(sb => sb.User)
                .Include(sb => sb.CashMovements)
                .FirstOrDefaultAsync(sb => sb.Id == id);

            if (smallBox == null)
            {
                return NotFound(new { message = "Caja chica no encontrada" });
            }

            var totalIncome = smallBox.CashMovements?.Where(cm => cm.MovementType == "Ingreso").Sum(cm => cm.Amount) ?? 0;
            var totalExpense = smallBox.CashMovements?.Where(cm => cm.MovementType == "Egreso").Sum(cm => cm.Amount) ?? 0;

            var response = new SmallBoxResponseDto
            {
                Id = smallBox.Id,
                InitialAmount = smallBox.InitialAmount,
                FinalAmount = smallBox.FinalAmount,
                OpeningDate = smallBox.OpeningDate,
                ClosingDate = smallBox.ClosingDate,
                AdditionalNote = smallBox.AdditionalNote,
                IsClosed = smallBox.IsClosed,
                UserId = smallBox.UserId,
                UserName = smallBox.User != null ? $"{smallBox.User.NameUser} {smallBox.User.LastNameUser}" : null,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                CurrentBalance = smallBox.InitialAmount + totalIncome - totalExpense,
                CashMovements = smallBox.CashMovements?.Select(cm => new CashMovementDto
                {
                    Id = cm.Id,
                    MovementType = cm.MovementType,
                    Amount = cm.Amount,
                    Concept = cm.Concept,
                    MovementDate = cm.MovementDate
                }).ToList() ?? new List<CashMovementDto>(),
                CreatedAt = smallBox.CreatedAt,
                UpdatedAt = smallBox.UpdatedAt
            };

            return Ok(response);
        }

        [HttpGet("active")]
        public async Task<ActionResult<SmallBoxResponseDto>> GetActiveSmallBox()
        {
            var smallBox = await _context.SmallBoxes
                .Include(sb => sb.User)
                .Include(sb => sb.CashMovements)
                .Where(sb => !sb.IsClosed)
                .OrderByDescending(sb => sb.OpeningDate)
                .FirstOrDefaultAsync();

            if (smallBox == null)
            {
                return NotFound(new { message = "No hay caja chica activa" });
            }

            var totalIncome = smallBox.CashMovements?.Where(cm => cm.MovementType == "Ingreso").Sum(cm => cm.Amount) ?? 0;
            var totalExpense = smallBox.CashMovements?.Where(cm => cm.MovementType == "Egreso").Sum(cm => cm.Amount) ?? 0;

            var response = new SmallBoxResponseDto
            {
                Id = smallBox.Id,
                InitialAmount = smallBox.InitialAmount,
                FinalAmount = smallBox.FinalAmount,
                OpeningDate = smallBox.OpeningDate,
                ClosingDate = smallBox.ClosingDate,
                AdditionalNote = smallBox.AdditionalNote,
                IsClosed = smallBox.IsClosed,
                UserId = smallBox.UserId,
                UserName = smallBox.User != null ? $"{smallBox.User.NameUser} {smallBox.User.LastNameUser}" : null,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                CurrentBalance = smallBox.InitialAmount + totalIncome - totalExpense,
                CashMovements = smallBox.CashMovements?.Select(cm => new CashMovementDto
                {
                    Id = cm.Id,
                    MovementType = cm.MovementType,
                    Amount = cm.Amount,
                    Concept = cm.Concept,
                    MovementDate = cm.MovementDate
                }).OrderByDescending(cm => cm.MovementDate).ToList() ?? new List<CashMovementDto>(),
                CreatedAt = smallBox.CreatedAt,
                UpdatedAt = smallBox.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<SmallBoxResponseDto>> OpenSmallBox([FromBody] CreateSmallBoxDto createSmallBoxDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar si existe una caja abierta
            var activeSmallBox = await _context.SmallBoxes
                .Where(sb => !sb.IsClosed)
                .FirstOrDefaultAsync();

            if (activeSmallBox != null)
            {
                return BadRequest(new { message = "Ya existe una caja chica abierta. Debe cerrarla antes de abrir una nueva." });
            }

            // Obtener usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

            var smallBox = new Models.SmallBox
            {
                InitialAmount = createSmallBoxDto.InitialAmount,
                FinalAmount = 0,
                OpeningDate = DateTime.UtcNow,
                AdditionalNote = createSmallBoxDto.AdditionalNote,
                IsClosed = false,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.SmallBoxes.Add(smallBox);
            await _context.SaveChangesAsync();

            await _context.Entry(smallBox).Reference(sb => sb.User).LoadAsync();

            var response = new SmallBoxResponseDto
            {
                Id = smallBox.Id,
                InitialAmount = smallBox.InitialAmount,
                FinalAmount = smallBox.FinalAmount,
                OpeningDate = smallBox.OpeningDate,
                ClosingDate = smallBox.ClosingDate,
                AdditionalNote = smallBox.AdditionalNote,
                IsClosed = smallBox.IsClosed,
                UserId = smallBox.UserId,
                UserName = smallBox.User != null ? $"{smallBox.User.NameUser} {smallBox.User.LastNameUser}" : null,
                TotalIncome = 0,
                TotalExpense = 0,
                CurrentBalance = smallBox.InitialAmount,
                CashMovements = new List<CashMovementDto>(),
                CreatedAt = smallBox.CreatedAt,
                UpdatedAt = smallBox.UpdatedAt
            };

            return CreatedAtAction(nameof(GetSmallBox), new { id = smallBox.Id }, response);
        }

        [HttpPatch("{id}/close")]
        public async Task<ActionResult<SmallBoxResponseDto>> CloseSmallBox(int id, [FromBody] CloseSmallBoxDto closeSmallBoxDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var smallBox = await _context.SmallBoxes
                .Include(sb => sb.User)
                .Include(sb => sb.CashMovements)
                .FirstOrDefaultAsync(sb => sb.Id == id);

            if (smallBox == null)
            {
                return NotFound(new { message = "Caja chica no encontrada" });
            }

            if (smallBox.IsClosed)
            {
                return BadRequest(new { message = "La caja chica ya está cerrada" });
            }

            smallBox.FinalAmount = closeSmallBoxDto.FinalAmount;
            smallBox.ClosingDate = DateTime.UtcNow;
            smallBox.IsClosed = true;
            smallBox.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(closeSmallBoxDto.AdditionalNote))
            {
                smallBox.AdditionalNote = closeSmallBoxDto.AdditionalNote;
            }

            await _context.SaveChangesAsync();

            var totalIncome = smallBox.CashMovements?.Where(cm => cm.MovementType == "Ingreso").Sum(cm => cm.Amount) ?? 0;
            var totalExpense = smallBox.CashMovements?.Where(cm => cm.MovementType == "Egreso").Sum(cm => cm.Amount) ?? 0;

            var response = new SmallBoxResponseDto
            {
                Id = smallBox.Id,
                InitialAmount = smallBox.InitialAmount,
                FinalAmount = smallBox.FinalAmount,
                OpeningDate = smallBox.OpeningDate,
                ClosingDate = smallBox.ClosingDate,
                AdditionalNote = smallBox.AdditionalNote,
                IsClosed = smallBox.IsClosed,
                UserId = smallBox.UserId,
                UserName = smallBox.User != null ? $"{smallBox.User.NameUser} {smallBox.User.LastNameUser}" : null,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                CurrentBalance = smallBox.InitialAmount + totalIncome - totalExpense,
                CashMovements = smallBox.CashMovements?.Select(cm => new CashMovementDto
                {
                    Id = cm.Id,
                    MovementType = cm.MovementType,
                    Amount = cm.Amount,
                    Concept = cm.Concept,
                    MovementDate = cm.MovementDate
                }).ToList() ?? new List<CashMovementDto>(),
                CreatedAt = smallBox.CreatedAt,
                UpdatedAt = smallBox.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPost("cash-movement")]
        public async Task<ActionResult<CashMovementDto>> CreateCashMovement([FromBody] CreateCashMovementDto createCashMovementDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que el tipo sea Ingreso o Egreso
            if (createCashMovementDto.MovementType != "Ingreso" && createCashMovementDto.MovementType != "Egreso")
            {
                return BadRequest(new { message = "El tipo de movimiento debe ser 'Ingreso' o 'Egreso'" });
            }

            // Verificar que la caja existe y está abierta
            var smallBox = await _context.SmallBoxes.FindAsync(createCashMovementDto.SmallBoxId);
            if (smallBox == null)
            {
                return BadRequest(new { message = "La caja chica especificada no existe" });
            }

            if (smallBox.IsClosed)
            {
                return BadRequest(new { message = "No se pueden agregar movimientos a una caja cerrada" });
            }

            var cashMovement = new CashMovement
            {
                MovementType = createCashMovementDto.MovementType,
                Amount = createCashMovementDto.Amount,
                Concept = createCashMovementDto.Concept,
                MovementDate = DateTime.UtcNow,
                SmallBoxId = createCashMovementDto.SmallBoxId,
                CreatedAt = DateTime.UtcNow
            };

            _context.CashMovements.Add(cashMovement);
            await _context.SaveChangesAsync();

            var response = new CashMovementDto
            {
                Id = cashMovement.Id,
                MovementType = cashMovement.MovementType,
                Amount = cashMovement.Amount,
                Concept = cashMovement.Concept,
                MovementDate = cashMovement.MovementDate
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSmallBox(int id)
        {
            var smallBox = await _context.SmallBoxes
                .Include(sb => sb.CashMovements)
                .FirstOrDefaultAsync(sb => sb.Id == id);

            if (smallBox == null)
            {
                return NotFound(new { message = "Caja chica no encontrada" });
            }

            if (!smallBox.IsClosed)
            {
                return BadRequest(new { message = "No se puede eliminar una caja chica que está abierta. Debe cerrarla primero." });
            }

            // Eliminar movimientos de caja asociados
            if (smallBox.CashMovements != null && smallBox.CashMovements.Any())
            {
                _context.CashMovements.RemoveRange(smallBox.CashMovements);
            }

            _context.SmallBoxes.Remove(smallBox);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Caja chica eliminada exitosamente" });
        }
    }
}
