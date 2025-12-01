using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.RegistersDTOs.PaymentMethods;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Restaurant.Controllers.Registers.PaymentMethods
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly AppData _context;

        public PaymentMethodsController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous] // Permitir acceso sin autenticación para que el frontend pueda cargar los métodos
        public async Task<ActionResult<PaginatedPaymentMethodsResponseDto>> GetPaymentMethods(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 50;

            var query = _context.PaymentMethods.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(pm =>
                    pm.NamePaymentMethod.ToLower().Contains(search) ||
                    (pm.Description != null && pm.Description.ToLower().Contains(search)));
            }

            var total = await query.CountAsync();

            var paymentMethods = await query
                .OrderBy(pm => pm.NamePaymentMethod)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(pm => new PaymentMethodResponseDto
                {
                    Id = pm.Id,
                    Name = pm.NamePaymentMethod,
                    Description = pm.Description,
                    RequiresAuthorization = pm.RequiresAuthorization,
                    CreatedAt = pm.CreatedAt,
                    UpdatedAt = pm.UpdatedAt
                })
                .ToListAsync();

            var response = new PaginatedPaymentMethodsResponseDto
            {
                PaymentMethods = paymentMethods,
                Total = total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PaymentMethodResponseDto>> GetPaymentMethod(int id)
        {
            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == id);

            if (paymentMethod == null)
            {
                return NotFound(new { message = "Método de pago no encontrado" });
            }

            var response = new PaymentMethodResponseDto
            {
                Id = paymentMethod.Id,
                Name = paymentMethod.NamePaymentMethod,
                Description = paymentMethod.Description,
                RequiresAuthorization = paymentMethod.RequiresAuthorization,
                CreatedAt = paymentMethod.CreatedAt,
                UpdatedAt = paymentMethod.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<PaymentMethodResponseDto>> CreatePaymentMethod([FromBody] CreatePaymentMethodDto createPaymentMethodDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que no exista un método de pago con el mismo nombre
            if (await _context.PaymentMethods.AnyAsync(pm => pm.NamePaymentMethod.ToLower() == createPaymentMethodDto.Name.ToLower()))
            {
                return BadRequest(new { message = "Ya existe un método de pago con este nombre" });
            }

            var paymentMethod = new PaymentMethod
            {
                NamePaymentMethod = createPaymentMethodDto.Name,
                Description = createPaymentMethodDto.Description,
                RequiresAuthorization = createPaymentMethodDto.RequiresAuthorization,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentMethods.Add(paymentMethod);
            await _context.SaveChangesAsync();

            var response = new PaymentMethodResponseDto
            {
                Id = paymentMethod.Id,
                Name = paymentMethod.NamePaymentMethod,
                Description = paymentMethod.Description,
                RequiresAuthorization = paymentMethod.RequiresAuthorization,
                CreatedAt = paymentMethod.CreatedAt,
                UpdatedAt = paymentMethod.UpdatedAt
            };

            return CreatedAtAction(nameof(GetPaymentMethod), new { id = paymentMethod.Id }, response);
        }

        [HttpPost("seed")]
        public async Task<ActionResult> SeedPaymentMethods()
        {
            // Verificar si ya existen métodos de pago
            if (await _context.PaymentMethods.AnyAsync())
            {
                return BadRequest(new { message = "Ya existen métodos de pago en la base de datos" });
            }

            var paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod
                {
                    NamePaymentMethod = "Efectivo",
                    Description = "Pago en efectivo",
                    RequiresAuthorization = false,
                    CreatedAt = DateTime.UtcNow
                },
                new PaymentMethod
                {
                    NamePaymentMethod = "Yape",
                    Description = "Pago mediante Yape",
                    RequiresAuthorization = false,
                    CreatedAt = DateTime.UtcNow
                },
                new PaymentMethod
                {
                    NamePaymentMethod = "Plin",
                    Description = "Pago mediante Plin",
                    RequiresAuthorization = false,
                    CreatedAt = DateTime.UtcNow
                },
                new PaymentMethod
                {
                    NamePaymentMethod = "Tarjeta Crédito",
                    Description = "Pago con tarjeta de crédito",
                    RequiresAuthorization = true,
                    CreatedAt = DateTime.UtcNow
                },
                new PaymentMethod
                {
                    NamePaymentMethod = "Tarjeta Débito",
                    Description = "Pago con tarjeta de débito",
                    RequiresAuthorization = true,
                    CreatedAt = DateTime.UtcNow
                },
                new PaymentMethod
                {
                    NamePaymentMethod = "Transferencia Bancaria",
                    Description = "Pago mediante transferencia bancaria",
                    RequiresAuthorization = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.PaymentMethods.AddRange(paymentMethods);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Métodos de pago cargados exitosamente",
                count = paymentMethods.Count,
                paymentMethods = paymentMethods.Select(pm => new
                {
                    id = pm.Id,
                    name = pm.NamePaymentMethod,
                    description = pm.Description,
                    requiresAuthorization = pm.RequiresAuthorization
                })
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentMethodResponseDto>> UpdatePaymentMethod(int id, [FromBody] UpdatePaymentMethodDto updatePaymentMethodDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paymentMethod = await _context.PaymentMethods.FindAsync(id);

            if (paymentMethod == null)
            {
                return NotFound(new { message = "Método de pago no encontrado" });
            }

            // Validar que no exista otro método de pago con el mismo nombre
            if (await _context.PaymentMethods.AnyAsync(pm => pm.NamePaymentMethod.ToLower() == updatePaymentMethodDto.Name.ToLower() && pm.Id != id))
            {
                return BadRequest(new { message = "Ya existe otro método de pago con este nombre" });
            }

            paymentMethod.NamePaymentMethod = updatePaymentMethodDto.Name;
            paymentMethod.Description = updatePaymentMethodDto.Description;
            paymentMethod.RequiresAuthorization = updatePaymentMethodDto.RequiresAuthorization;
            paymentMethod.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PaymentMethodExists(id))
                {
                    return NotFound(new { message = "Método de pago no encontrado" });
                }
                else
                {
                    throw;
                }
            }

            var response = new PaymentMethodResponseDto
            {
                Id = paymentMethod.Id,
                Name = paymentMethod.NamePaymentMethod,
                Description = paymentMethod.Description,
                RequiresAuthorization = paymentMethod.RequiresAuthorization,
                CreatedAt = paymentMethod.CreatedAt,
                UpdatedAt = paymentMethod.UpdatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);

            if (paymentMethod == null)
            {
                return NotFound(new { message = "Método de pago no encontrado" });
            }

            // Verificar si el método de pago está siendo usado en pedidos
            var hasOrders = await _context.Orders.AnyAsync(o => o.PaymentMethodId == id);

            if (hasOrders)
            {
                return BadRequest(new
                {
                    message = "No se puede eliminar el método de pago porque está siendo usado en pedidos existentes"
                });
            }

            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Método de pago eliminado exitosamente" });
        }

        private async Task<bool> PaymentMethodExists(int id)
        {
            return await _context.PaymentMethods.AnyAsync(e => e.Id == id);
        }
    }
}
