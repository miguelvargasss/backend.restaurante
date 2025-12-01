using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Workers;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Restaurant.Controllers.Workers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkersController : ControllerBase
    {
        private readonly AppData _context;

        public WorkersController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedWorkersResponseDto>> GetWorkers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Workers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(w =>
                    w.NameWorker.ToLower().Contains(search) ||
                    w.LastNameWorker.ToLower().Contains(search) ||
                    w.DNI.ToString().Contains(search) ||
                    w.EmailWorker != null && w.EmailWorker.ToLower().Contains(search));
            }

            var total = await query.CountAsync();

            var workers = await query
                .OrderByDescending(w => w.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WorkerResponseDto
                {
                    Id = w.Id,
                    Name = w.NameWorker,
                    LastName = w.LastNameWorker,
                    DNI = w.DNI,
                    Phone = w.PhoneWorker,
                    Email = w.EmailWorker,
                    Salary = w.SalaryWorker,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .ToListAsync();

            var response = new PaginatedWorkersResponseDto
            {
                Workers = workers,
                Total = total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkerResponseDto>> GetWorker(int id)
        {
            var worker = await _context.Workers
                .FirstOrDefaultAsync(w => w.Id == id);

            if (worker == null)
            {
                return NotFound(new { message = "Trabajador no encontrado" });
            }

            var response = new WorkerResponseDto
            {
                Id = worker.Id,
                Name = worker.NameWorker,
                LastName = worker.LastNameWorker,
                DNI = worker.DNI,
                Phone = worker.PhoneWorker,
                Email = worker.EmailWorker,
                Salary = worker.SalaryWorker,
                CreatedAt = worker.CreatedAt,
                UpdatedAt = worker.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<WorkerResponseDto>> CreateWorker([FromBody] CreateWorkerDto createWorkerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Workers.AnyAsync(w => w.DNI == createWorkerDto.DNI))
            {
                return BadRequest(new { message = "Ya existe un trabajador con este DNI" });
            }

            if (!string.IsNullOrWhiteSpace(createWorkerDto.Email) &&
                await _context.Workers.AnyAsync(w => w.EmailWorker == createWorkerDto.Email))
            {
                return BadRequest(new { message = "El email ya está registrado" });
            }

            var worker = new Worker
            {
                NameWorker = createWorkerDto.Name,
                LastNameWorker = createWorkerDto.LastName,
                DNI = createWorkerDto.DNI,
                PhoneWorker = createWorkerDto.Phone,
                EmailWorker = createWorkerDto.Email,
                SalaryWorker = createWorkerDto.Salary,
                CreatedAt = DateTime.UtcNow
            };

            _context.Workers.Add(worker);
            await _context.SaveChangesAsync();

            var response = new WorkerResponseDto
            {
                Id = worker.Id,
                Name = worker.NameWorker,
                LastName = worker.LastNameWorker,
                DNI = worker.DNI,
                Phone = worker.PhoneWorker,
                Email = worker.EmailWorker,
                Salary = worker.SalaryWorker,
                CreatedAt = worker.CreatedAt,
                UpdatedAt = worker.UpdatedAt
            };

            return CreatedAtAction(nameof(GetWorker), new { id = worker.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WorkerResponseDto>> UpdateWorker(int id, [FromBody] UpdateWorkerDto updateWorkerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var worker = await _context.Workers.FindAsync(id);

            if (worker == null)
            {
                return NotFound(new { message = "Trabajador no encontrado" });
            }

            if (await _context.Workers.AnyAsync(w => w.DNI == updateWorkerDto.DNI && w.Id != id))
            {
                return BadRequest(new { message = "Ya existe otro trabajador con este DNI" });
            }

            if (!string.IsNullOrWhiteSpace(updateWorkerDto.Email) &&
                await _context.Workers.AnyAsync(w => w.EmailWorker == updateWorkerDto.Email && w.Id != id))
            {
                return BadRequest(new { message = "El email ya está registrado en otro trabajador" });
            }

            worker.NameWorker = updateWorkerDto.Name;
            worker.LastNameWorker = updateWorkerDto.LastName;
            worker.DNI = updateWorkerDto.DNI;
            worker.PhoneWorker = updateWorkerDto.Phone;
            worker.EmailWorker = updateWorkerDto.Email;
            worker.SalaryWorker = updateWorkerDto.Salary;
            worker.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await WorkerExists(id))
                {
                    return NotFound(new { message = "Trabajador no encontrado" });
                }
                else
                {
                    throw;
                }
            }

            var response = new WorkerResponseDto
            {
                Id = worker.Id,
                Name = worker.NameWorker,
                LastName = worker.LastNameWorker,
                DNI = worker.DNI,
                Phone = worker.PhoneWorker,
                Email = worker.EmailWorker,
                Salary = worker.SalaryWorker,
                CreatedAt = worker.CreatedAt,
                UpdatedAt = worker.UpdatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            var worker = await _context.Workers.FindAsync(id);

            if (worker == null)
            {
                return NotFound(new { message = "Trabajador no encontrado" });
            }

            var hasOrders = await _context.Orders.AnyAsync(o => o.WorkerId == id);
            var hasReserves = await _context.Reserves.AnyAsync(r => r.WorkerId == id);

            if (hasOrders || hasReserves)
            {
                return BadRequest(new
                {
                    message = "No se puede eliminar el trabajador porque tiene registros asociados (órdenes o reservas)."
                });
            }

            _context.Workers.Remove(worker);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Trabajador eliminado exitosamente" });
        }

        private async Task<bool> WorkerExists(int id)
        {
            return await _context.Workers.AnyAsync(e => e.Id == id);
        }
    }
}
