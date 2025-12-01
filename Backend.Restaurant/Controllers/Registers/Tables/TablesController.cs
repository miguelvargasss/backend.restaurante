using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Registers.Tables;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Restaurant.Controllers.Registers.Tables
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TablesController : ControllerBase
    {
        private readonly AppData _context;

        public TablesController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedTablesResponseDto>> GetTables(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? loungeId = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Tables
                .Include(t => t.Lounge)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(t =>
                    t.NameTable.ToLower().Contains(search) ||
                    t.Environment.ToLower().Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(t => t.IsActive == isActive.Value);
            }

            if (loungeId.HasValue)
            {
                query = query.Where(t => t.LoungeId == loungeId.Value);
            }

            var total = await query.CountAsync();

            var tables = await query
                .OrderBy(t => t.NameTable)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Obtener IDs de las mesas
            var tableIds = tables.Select(t => t.Id).ToList();

            // Obtener los pedidos activos de estas mesas en una sola consulta
            var activeOrders = await _context.Orders
                .Include(o => o.User)
                .Where(o => tableIds.Contains(o.TableId.GetValueOrDefault()) && !o.IsPaid && o.Status != "Cancelada")
                .ToListAsync();

            // Mapear a DTO
            var tableResponseDtos = tables.Select(t =>
            {
                var currentOrder = activeOrders
                    .Where(o => o.TableId == t.Id)
                    .OrderByDescending(o => o.OrderDate)
                    .FirstOrDefault();

                return new TableResponseDto
                {
                    Id = t.Id,
                    Name = t.NameTable,
                    Environment = t.Environment,
                    Capacity = t.Capacity,
                    IsActive = t.IsActive,
                    LoungeId = t.LoungeId,
                    LoungeName = t.Lounge?.NameLounge,
                    
                    // Campos de ocupación simplificados
                    IsOccupied = currentOrder != null,
                    OccupiedBy = currentOrder?.User?.NameUser,
                    OccupiedByUserId = currentOrder?.UserId,
                    CurrentOrderId = currentOrder?.Id,
                    CurrentOrderStatus = currentOrder?.Status,
                    CurrentOrderIsPaid = currentOrder?.IsPaid,
                    
                    // Información completa del pedido actual
                    CurrentOrder = currentOrder != null ? new CurrentOrderDto
                    {
                        Id = currentOrder.Id,
                        UserId = currentOrder.UserId,
                        UserName = currentOrder.User?.NameUser,
                        Status = currentOrder.Status,
                        IsPaid = currentOrder.IsPaid
                    } : null,
                    
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                };
            }).ToList();

            return Ok(new PaginatedTablesResponseDto
            {
                Tables = tableResponseDtos,
                Total = total,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("simple")]
        public async Task<IActionResult> GetSimpleTables()
        {
            var tables = await _context.Tables
                .Where(t => t.IsActive)
                .OrderBy(t => t.NameTable)
                .Select(t => new
                {
                    id = t.Id,
                    name = t.NameTable,
                    capacity = t.Capacity,
                    environment = t.Environment
                })
                .ToListAsync();

            return Ok(tables);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TableResponseDto>> GetTable(int id)
        {
            var table = await _context.Tables
                .Include(t => t.Lounge)
                .Include(t => t.Orders.Where(o => !o.IsPaid && o.Status != "Cancelada"))
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (table == null)
            {
                return NotFound(new { message = "Mesa no encontrada" });
            }

            var currentOrder = table.Orders?
                .Where(o => !o.IsPaid && o.Status != "Cancelada")
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefault();

            return Ok(new TableResponseDto
            {
                Id = table.Id,
                Name = table.NameTable,
                Environment = table.Environment,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                LoungeId = table.LoungeId,
                LoungeName = table.Lounge?.NameLounge,
                
                // ? Campos de ocupación
                IsOccupied = currentOrder != null,
                OccupiedBy = currentOrder?.User?.NameUser,
                OccupiedByUserId = currentOrder?.UserId,
                CurrentOrderId = currentOrder?.Id,
                CurrentOrderStatus = currentOrder?.Status,
                CurrentOrderIsPaid = currentOrder?.IsPaid,
                
                CurrentOrder = currentOrder != null ? new CurrentOrderDto
                {
                    Id = currentOrder.Id,
                    UserId = currentOrder.UserId,
                    UserName = currentOrder.User?.NameUser,
                    Status = currentOrder.Status,
                    IsPaid = currentOrder.IsPaid
                } : null,
                
                CreatedAt = table.CreatedAt,
                UpdatedAt = table.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<TableResponseDto>> CreateTable([FromBody] CreateTableDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.LoungeId.HasValue)
            {
                var lounge = await _context.Lounges.FindAsync(dto.LoungeId.Value);
                if (lounge == null)
                {
                    return BadRequest(new { message = "El salón especificado no existe" });
                }
            }

            if (await _context.Tables.AnyAsync(t => t.NameTable == dto.Name))
            {
                return BadRequest(new { message = "Ya existe una mesa con ese nombre" });
            }

            var table = new Table
            {
                NameTable = dto.Name,
                Environment = dto.Environment,
                Capacity = dto.Capacity,
                LoungeId = dto.LoungeId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            await _context.Entry(table).Reference(t => t.Lounge).LoadAsync();

            return CreatedAtAction(nameof(GetTable), new { id = table.Id }, new TableResponseDto
            {
                Id = table.Id,
                Name = table.NameTable,
                Environment = table.Environment,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                LoungeId = table.LoungeId,
                LoungeName = table.Lounge?.NameLounge,
                
                // Nueva mesa no está ocupada
                IsOccupied = false,
                OccupiedBy = null,
                OccupiedByUserId = null,
                CurrentOrderId = null,
                CurrentOrderStatus = null,
                CurrentOrderIsPaid = null,
                CurrentOrder = null,
                
                CreatedAt = table.CreatedAt,
                UpdatedAt = table.UpdatedAt
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TableResponseDto>> UpdateTable(int id, [FromBody] UpdateTableDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var table = await _context.Tables
                .Include(t => t.Lounge)
                .Include(t => t.Orders.Where(o => !o.IsPaid && o.Status != "Cancelada"))
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (table == null)
            {
                return NotFound(new { message = "Mesa no encontrada" });
            }

            if (dto.LoungeId.HasValue)
            {
                var lounge = await _context.Lounges.FindAsync(dto.LoungeId.Value);
                if (lounge == null)
                {
                    return BadRequest(new { message = "El salón especificado no existe" });
                }
            }

            if (await _context.Tables.AnyAsync(t => t.NameTable == dto.Name && t.Id != id))
            {
                return BadRequest(new { message = "Ya existe otra mesa con ese nombre" });
            }

            table.NameTable = dto.Name;
            table.Environment = dto.Environment;
            table.Capacity = dto.Capacity;
            table.LoungeId = dto.LoungeId;
            table.IsActive = dto.IsActive;
            table.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var currentOrder = table.Orders?
                .Where(o => !o.IsPaid && o.Status != "Cancelada")
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefault();

            return Ok(new TableResponseDto
            {
                Id = table.Id,
                Name = table.NameTable,
                Environment = table.Environment,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                LoungeId = table.LoungeId,
                LoungeName = table.Lounge?.NameLounge,
                
                // ? Campos de ocupación
                IsOccupied = currentOrder != null,
                OccupiedBy = currentOrder?.User?.NameUser,
                OccupiedByUserId = currentOrder?.UserId,
                CurrentOrderId = currentOrder?.Id,
                CurrentOrderStatus = currentOrder?.Status,
                CurrentOrderIsPaid = currentOrder?.IsPaid,
                
                CurrentOrder = currentOrder != null ? new CurrentOrderDto
                {
                    Id = currentOrder.Id,
                    UserId = currentOrder.UserId,
                    UserName = currentOrder.User?.NameUser,
                    Status = currentOrder.Status,
                    IsPaid = currentOrder.IsPaid
                } : null,
                
                CreatedAt = table.CreatedAt,
                UpdatedAt = table.UpdatedAt
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);

            if (table == null)
            {
                return NotFound(new { message = "Mesa no encontrada" });
            }

            var hasOrders = await _context.Orders.AnyAsync(o => o.TableId == id);
            var hasReserves = await _context.Reserves.AnyAsync(r => r.TableId == id);

            if (hasOrders || hasReserves)
            {
                return BadRequest(new
                {
                    message = "No se puede eliminar la mesa porque tiene órdenes o reservas asociadas. " +
                              "Considere desactivar la mesa en su lugar."
                });
            }

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Mesa eliminada exitosamente" });
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<TableResponseDto>> ToggleTableStatus(int id)
        {
            var table = await _context.Tables
                .Include(t => t.Lounge)
                .Include(t => t.Orders.Where(o => !o.IsPaid && o.Status != "Cancelada"))
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (table == null)
            {
                return NotFound(new { message = "Mesa no encontrada" });
            }

            table.IsActive = !table.IsActive;
            table.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var currentOrder = table.Orders?
                .Where(o => !o.IsPaid && o.Status != "Cancelada")
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefault();

            return Ok(new TableResponseDto
            {
                Id = table.Id,
                Name = table.NameTable,
                Environment = table.Environment,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                LoungeId = table.LoungeId,
                LoungeName = table.Lounge?.NameLounge,
                
                // ? Campos de ocupación
                IsOccupied = currentOrder != null,
                OccupiedBy = currentOrder?.User?.NameUser,
                OccupiedByUserId = currentOrder?.UserId,
                CurrentOrderId = currentOrder?.Id,
                CurrentOrderStatus = currentOrder?.Status,
                CurrentOrderIsPaid = currentOrder?.IsPaid,
                
                CurrentOrder = currentOrder != null ? new CurrentOrderDto
                {
                    Id = currentOrder.Id,
                    UserId = currentOrder.UserId,
                    UserName = currentOrder.User?.NameUser,
                    Status = currentOrder.Status,
                    IsPaid = currentOrder.IsPaid
                } : null,
                
                CreatedAt = table.CreatedAt,
                UpdatedAt = table.UpdatedAt
            });
        }
    }
}
