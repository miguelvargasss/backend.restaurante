using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Registers.Reserves;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Restaurant.Controllers.Registers.Reserves
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservesController : ControllerBase
    {
        private readonly AppData _context;

        public ReservesController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedReservesResponseDto>> GetReserves(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Reserves
                .Include(r => r.Table)
                .Include(r => r.Worker)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(r =>
                    r.NameUserReserve.ToLower().Contains(search) ||
                    r.PhoneReserve.Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(r => r.IsActive == isActive.Value);
            }

            if (startDate.HasValue)
            {
                // Convertir a UTC si no lo está
                var startDateUtc = startDate.Value.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc) 
                    : startDate.Value.ToUniversalTime();
                
                query = query.Where(r => r.ReservationDate >= startDateUtc);
            }

            if (endDate.HasValue)
            {
                // ✅ Busca hasta las 23:59:59 del día seleccionado
                var endDateUtc = endDate.Value.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(endDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc) 
                    : endDate.Value.Date.AddDays(1).AddTicks(-1).ToUniversalTime();
                
                query = query.Where(r => r.ReservationDate <= endDateUtc);
            }

            var total = await query.CountAsync();

            var reserves = await query
                .OrderByDescending(r => r.ReservationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReserveResponseDto
                {
                    Id = r.Id,
                    CustomerName = r.NameUserReserve,
                    Phone = r.PhoneReserve,
                    NumberOfPeople = r.NumberPeople,
                    AdvancePayment = r.AdvancePayment ?? false,
                    Amount = r.Amount,
                    ReservationDate = r.ReservationDate,
                    IsActive = r.IsActive,
                    TableId = r.TableId,
                    TableName = r.Table != null ? r.Table.NameTable : null,
                    WorkerId = r.WorkerId,
                    WorkerName = r.Worker != null ? r.Worker.NameWorker : null,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync();

            return Ok(new PaginatedReservesResponseDto
            {
                Reserves = reserves,
                Total = total,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReserveResponseDto>> GetReserve(int id)
        {
            var reserve = await _context.Reserves
                .Include(r => r.Table)
                .Include(r => r.Worker)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserve == null)
            {
                return NotFound(new { message = "Reserva no encontrada" });
            }

            return Ok(new ReserveResponseDto
            {
                Id = reserve.Id,
                CustomerName = reserve.NameUserReserve,
                Phone = reserve.PhoneReserve,
                NumberOfPeople = reserve.NumberPeople,
                AdvancePayment = reserve.AdvancePayment ?? false,
                Amount = reserve.Amount,
                ReservationDate = reserve.ReservationDate,
                IsActive = reserve.IsActive,
                TableId = reserve.TableId,
                TableName = reserve.Table?.NameTable,
                WorkerId = reserve.WorkerId,
                WorkerName = reserve.Worker?.NameWorker,
                CreatedAt = reserve.CreatedAt,
                UpdatedAt = reserve.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<ReserveResponseDto>> CreateReserve([FromBody] CreateReserveDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.TableId.HasValue)
            {
                var table = await _context.Tables.FindAsync(dto.TableId.Value);
                if (table == null)
                {
                    return BadRequest(new { message = "La mesa especificada no existe" });
                }
            }

            if (dto.WorkerId.HasValue)
            {
                var worker = await _context.Workers.FindAsync(dto.WorkerId.Value);
                if (worker == null)
                {
                    return BadRequest(new { message = "El trabajador especificado no existe" });
                }
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            if (int.TryParse(userIdClaim, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var reserve = new Reserve
            {
                NameUserReserve = dto.CustomerName,
                PhoneReserve = dto.Phone,
                NumberPeople = dto.NumberOfPeople,
                AdvancePayment = dto.AdvancePayment,
                Amount = dto.Amount,
                ReservationDate = dto.ReservationDate,
                TableId = dto.TableId,
                WorkerId = dto.WorkerId,
                UserId = userId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reserves.Add(reserve);
            await _context.SaveChangesAsync();

            await _context.Entry(reserve).Reference(r => r.Table).LoadAsync();
            await _context.Entry(reserve).Reference(r => r.Worker).LoadAsync();

            return CreatedAtAction(nameof(GetReserve), new { id = reserve.Id }, new ReserveResponseDto
            {
                Id = reserve.Id,
                CustomerName = reserve.NameUserReserve,
                Phone = reserve.PhoneReserve,
                NumberOfPeople = reserve.NumberPeople,
                AdvancePayment = reserve.AdvancePayment ?? false,
                Amount = reserve.Amount,
                ReservationDate = reserve.ReservationDate,
                IsActive = reserve.IsActive,
                TableId = reserve.TableId,
                TableName = reserve.Table?.NameTable,
                WorkerId = reserve.WorkerId,
                WorkerName = reserve.Worker?.NameWorker,
                CreatedAt = reserve.CreatedAt,
                UpdatedAt = reserve.UpdatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReserveResponseDto>> UpdateReserve(int id, [FromBody] UpdateReserveDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reserve = await _context.Reserves
                .Include(r => r.Table)
                .Include(r => r.Worker)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserve == null)
            {
                return NotFound(new { message = "Reserva no encontrada" });
            }

            if (dto.TableId.HasValue)
            {
                var table = await _context.Tables.FindAsync(dto.TableId.Value);
                if (table == null)
                {
                    return BadRequest(new { message = "La mesa especificada no existe" });
                }
            }

            if (dto.WorkerId.HasValue)
            {
                var worker = await _context.Workers.FindAsync(dto.WorkerId.Value);
                if (worker == null)
                {
                    return BadRequest(new { message = "El trabajador especificado no existe" });
                }
            }

            reserve.NameUserReserve = dto.CustomerName;
            reserve.PhoneReserve = dto.Phone;
            reserve.NumberPeople = dto.NumberOfPeople;
            reserve.AdvancePayment = dto.AdvancePayment;
            reserve.Amount = dto.Amount;
            reserve.ReservationDate = dto.ReservationDate;
            reserve.TableId = dto.TableId;
            reserve.WorkerId = dto.WorkerId;
            reserve.IsActive = dto.IsActive;
            reserve.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _context.Entry(reserve).Reference(r => r.Table).LoadAsync();
            await _context.Entry(reserve).Reference(r => r.Worker).LoadAsync();

            return Ok(new ReserveResponseDto
            {
                Id = reserve.Id,
                CustomerName = reserve.NameUserReserve,
                Phone = reserve.PhoneReserve,
                NumberOfPeople = reserve.NumberPeople,
                AdvancePayment = reserve.AdvancePayment ?? false,
                Amount = reserve.Amount,
                ReservationDate = reserve.ReservationDate,
                IsActive = reserve.IsActive,
                TableId = reserve.TableId,
                TableName = reserve.Table?.NameTable,
                WorkerId = reserve.WorkerId,
                WorkerName = reserve.Worker?.NameWorker,
                CreatedAt = reserve.CreatedAt,
                UpdatedAt = reserve.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserve(int id)
        {
            var reserve = await _context.Reserves.FindAsync(id);

            if (reserve == null)
            {
                return NotFound(new { message = "Reserva no encontrada" });
            }

            _context.Reserves.Remove(reserve);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reserva eliminada exitosamente" });
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<ReserveResponseDto>> ToggleReserveStatus(int id)
        {
            var reserve = await _context.Reserves
                .Include(r => r.Table)
                .Include(r => r.Worker)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserve == null)
            {
                return NotFound(new { message = "Reserva no encontrada" });
            }

            reserve.IsActive = !reserve.IsActive;
            reserve.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ReserveResponseDto
            {
                Id = reserve.Id,
                CustomerName = reserve.NameUserReserve,
                Phone = reserve.PhoneReserve,
                NumberOfPeople = reserve.NumberPeople,
                AdvancePayment = reserve.AdvancePayment ?? false,
                Amount = reserve.Amount,
                ReservationDate = reserve.ReservationDate,
                IsActive = reserve.IsActive,
                TableId = reserve.TableId,
                TableName = reserve.Table?.NameTable,
                WorkerId = reserve.WorkerId,
                WorkerName = reserve.Worker?.NameWorker,
                CreatedAt = reserve.CreatedAt,
                UpdatedAt = reserve.UpdatedAt
            });
        }
    }
}
