using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Orders;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Restaurant.Controllers.Registers.Orders
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppData _context;

        public OrdersController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedOrdersResponseDto>> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] int? tableId = null,
            [FromQuery] bool? isPaid = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Worker)
                .Include(o => o.PaymentMethod)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(o =>
                    o.OrderNumber.ToLower().Contains(search) ||
                    (o.CustomerName != null && o.CustomerName.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            if (tableId.HasValue)
            {
                query = query.Where(o => o.TableId == tableId.Value);
            }

            if (isPaid.HasValue)
            {
                query = query.Where(o => o.IsPaid == isPaid.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= endDate.Value);
            }

            var total = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    UserId = o.UserId,
                    TableId = o.TableId,
                    TableName = o.Table != null ? o.Table.NameTable : null,
                    WorkerId = o.WorkerId,
                    WorkerName = o.Worker != null ? $"{o.Worker.NameWorker} {o.Worker.LastNameWorker}" : null,
                    PaymentMethodId = o.PaymentMethodId,
                    PaymentMethodName = o.PaymentMethod != null ? o.PaymentMethod.NamePaymentMethod : null,
                    SubTotal = o.SubTotal,
                    Discount = o.Discount,
                    Tax = o.Tax,
                    Total = o.Total,
                    CustomerName = o.CustomerName,
                    OrderType = o.OrderType,
                    Observations = o.Observations,
                    IsPaid = o.IsPaid,
                    CompletedAt = o.CompletedAt,
                    OrderDetails = o.OrderDetails!.Select(od => new OrderDetailDto
                    {
                        Id = od.Id,
                        ProductId = od.ProductId,
                        ProductName = od.ProductName,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice,
                        SubTotal = od.SubTotal,
                        Total = od.Total,
                        Observations = od.Observations,
                        Status = od.Status
                    }).ToList(),
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToListAsync();

            var response = new PaginatedOrdersResponseDto
            {
                Orders = orders,
                Total = total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Worker)
                .Include(o => o.PaymentMethod)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            var response = new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                UserId = order.UserId,
                TableId = order.TableId,
                TableName = order.Table?.NameTable,
                WorkerId = order.WorkerId,
                WorkerName = order.Worker != null ? $"{order.Worker.NameWorker} {order.Worker.LastNameWorker}" : null,
                PaymentMethodId = order.PaymentMethodId,
                PaymentMethodName = order.PaymentMethod?.NamePaymentMethod,
                SubTotal = order.SubTotal,
                Discount = order.Discount,
                Tax = order.Tax,
                Total = order.Total,
                CustomerName = order.CustomerName,
                OrderType = order.OrderType,
                Observations = order.Observations,
                IsPaid = order.IsPaid,
                CompletedAt = order.CompletedAt,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    SubTotal = od.SubTotal,
                    Total = od.Total,
                    Observations = od.Observations,
                    Status = od.Status
                }).ToList() ?? new List<OrderDetailDto>(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar mesa
            var table = await _context.Tables.FindAsync(createOrderDto.TableId);
            if (table == null)
            {
                return BadRequest(new { message = "La mesa especificada no existe" });
            }

            // Validar trabajador si se proporciona
            if (createOrderDto.WorkerId.HasValue)
            {
                var worker = await _context.Workers.FindAsync(createOrderDto.WorkerId.Value);
                if (worker == null)
                {
                    return BadRequest(new { message = "El trabajador especificado no existe" });
                }
            }

            // Validar método de pago
            var paymentMethod = await _context.PaymentMethods.FindAsync(createOrderDto.PaymentMethodId);
            if (paymentMethod == null)
            {
                return BadRequest(new { message = "El método de pago especificado no existe" });
            }

            // Validar productos
            foreach (var detail in createOrderDto.OrderDetails)
            {
                if (detail.ProductId.HasValue)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId.Value);
                    if (product == null)
                    {
                        return BadRequest(new { message = $"El producto con ID {detail.ProductId} no existe" });
                    }
                }
            }

            // Obtener usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

            // Calcular totales
            decimal subTotal = createOrderDto.OrderDetails.Sum(d => d.SubTotal);
            decimal total = subTotal - createOrderDto.Discount + createOrderDto.Tax;

            // Generar número de orden
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}";

            var order = new Order
            {
                OrderNumber = orderNumber,
                OrderDate = DateTime.UtcNow,
                Status = "Pendiente",
                UserId = userId,
                TableId = createOrderDto.TableId,
                WorkerId = createOrderDto.WorkerId,
                PaymentMethodId = createOrderDto.PaymentMethodId,
                SubTotal = subTotal,
                Discount = createOrderDto.Discount,
                Tax = createOrderDto.Tax,
                Total = total,
                CustomerName = createOrderDto.CustomerName,
                OrderType = createOrderDto.OrderType,
                Observations = createOrderDto.Observations,
                IsPaid = false, // ? El pedido se crea como NO PAGADO
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Crear detalles de la orden
            foreach (var detailDto in createOrderDto.OrderDetails)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = detailDto.ProductId,
                    ProductName = detailDto.ProductName,
                    Quantity = detailDto.Quantity,
                    UnitPrice = detailDto.UnitPrice,
                    SubTotal = detailDto.SubTotal,
                    Total = detailDto.Total,
                    Observations = detailDto.Observations,
                    Status = detailDto.Status,
                    CreatedAt = DateTime.UtcNow
                };

                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();

            // ? REMOVIDO: Ya NO se crea el movimiento de caja aquí
            // El movimiento de caja se creará solo cuando se marque el pedido como pagado

            // Recargar la orden con todas sus relaciones
            await _context.Entry(order)
                .Reference(o => o.Table).LoadAsync();
            await _context.Entry(order)
                .Reference(o => o.Worker).LoadAsync();
            await _context.Entry(order)
                .Reference(o => o.PaymentMethod).LoadAsync();
            await _context.Entry(order)
                .Collection(o => o.OrderDetails!).LoadAsync();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                UserId = order.UserId,
                TableId = order.TableId,
                TableName = order.Table?.NameTable,
                WorkerId = order.WorkerId,
                WorkerName = order.Worker != null ? $"{order.Worker.NameWorker} {order.Worker.LastNameWorker}" : null,
                PaymentMethodId = order.PaymentMethodId,
                PaymentMethodName = order.PaymentMethod?.NamePaymentMethod,
                SubTotal = order.SubTotal,
                Discount = order.Discount,
                Tax = order.Tax,
                Total = order.Total,
                CustomerName = order.CustomerName,
                OrderType = order.OrderType,
                Observations = order.Observations,
                IsPaid = order.IsPaid,
                CompletedAt = order.CompletedAt,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    SubTotal = od.SubTotal,
                    Total = od.Total,
                    Observations = od.Observations,
                    Status = od.Status
                }).ToList() ?? new List<OrderDetailDto>(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrderResponseDto>> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            // Validar mesa
            var table = await _context.Tables.FindAsync(updateOrderDto.TableId);
            if (table == null)
            {
                return BadRequest(new { message = "La mesa especificada no existe" });
            }

            // Validar trabajador si se proporciona
            if (updateOrderDto.WorkerId.HasValue)
            {
                var worker = await _context.Workers.FindAsync(updateOrderDto.WorkerId.Value);
                if (worker == null)
                {
                    return BadRequest(new { message = "El trabajador especificado no existe" });
                }
            }

            // Validar método de pago
            var paymentMethod = await _context.PaymentMethods.FindAsync(updateOrderDto.PaymentMethodId);
            if (paymentMethod == null)
            {
                return BadRequest(new { message = "El método de pago especificado no existe" });
            }

            // Actualizar orden
            order.TableId = updateOrderDto.TableId;
            order.WorkerId = updateOrderDto.WorkerId;
            order.PaymentMethodId = updateOrderDto.PaymentMethodId;
            order.Status = updateOrderDto.Status;
            order.CustomerName = updateOrderDto.CustomerName;
            order.OrderType = updateOrderDto.OrderType;
            order.Observations = updateOrderDto.Observations;
            order.Discount = updateOrderDto.Discount;
            order.Tax = updateOrderDto.Tax;
            order.IsPaid = updateOrderDto.IsPaid;
            order.UpdatedAt = DateTime.UtcNow;

            // Recalcular total
            order.Total = order.SubTotal - order.Discount + order.Tax;

            if (updateOrderDto.Status == "Completada" && !order.CompletedAt.HasValue)
            {
                order.CompletedAt = DateTime.UtcNow;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderExists(id))
                {
                    return NotFound(new { message = "Pedido no encontrado" });
                }
                else
                {
                    throw;
                }
            }

            // Recargar relaciones
            await _context.Entry(order).Reference(o => o.Table).LoadAsync();
            await _context.Entry(order).Reference(o => o.Worker).LoadAsync();
            await _context.Entry(order).Reference(o => o.PaymentMethod).LoadAsync();
            await _context.Entry(order).Collection(o => o.OrderDetails!).LoadAsync();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                UserId = order.UserId,
                TableId = order.TableId,
                TableName = order.Table?.NameTable,
                WorkerId = order.WorkerId,
                WorkerName = order.Worker != null ? $"{order.Worker.NameWorker} {order.Worker.LastNameWorker}" : null,
                PaymentMethodId = order.PaymentMethodId,
                PaymentMethodName = order.PaymentMethod?.NamePaymentMethod,
                SubTotal = order.SubTotal,
                Discount = order.Discount,
                Tax = order.Tax,
                Total = order.Total,
                CustomerName = order.CustomerName,
                OrderType = order.OrderType,
                Observations = order.Observations,
                IsPaid = order.IsPaid,
                CompletedAt = order.CompletedAt,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    SubTotal = od.SubTotal,
                    Total = od.Total,
                    Observations = od.Observations,
                    Status = od.Status
                }).ToList() ?? new List<OrderDetailDto>(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            if (order.IsPaid)
            {
                return BadRequest(new { message = "No se puede eliminar un pedido que ya fue pagado" });
            }

            // Eliminar detalles de la orden
            if (order.OrderDetails != null)
            {
                _context.OrderDetails.RemoveRange(order.OrderDetails);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pedido eliminado exitosamente" });
        }

        [HttpPatch("{id}/change-status")]
        public async Task<ActionResult<OrderResponseDto>> ChangeOrderStatus(int id, [FromBody] string newStatus)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Worker)
                .Include(o => o.PaymentMethod)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            var validStatuses = new[] { "Pendiente", "En Proceso", "Completada", "Cancelada" };
            if (!validStatuses.Contains(newStatus))
            {
                return BadRequest(new { message = "Estado inválido" });
            }

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            if (newStatus == "Completada" && !order.CompletedAt.HasValue)
            {
                order.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                UserId = order.UserId,
                TableId = order.TableId,
                TableName = order.Table?.NameTable,
                WorkerId = order.WorkerId,
                WorkerName = order.Worker != null ? $"{order.Worker.NameWorker} {order.Worker.LastNameWorker}" : null,
                PaymentMethodId = order.PaymentMethodId,
                PaymentMethodName = order.PaymentMethod?.NamePaymentMethod,
                SubTotal = order.SubTotal,
                Discount = order.Discount,
                Tax = order.Tax,
                Total = order.Total,
                CustomerName = order.CustomerName,
                OrderType = order.OrderType,
                Observations = order.Observations,
                IsPaid = order.IsPaid,
                CompletedAt = order.CompletedAt,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    SubTotal = od.SubTotal,
                    Total = od.Total,
                    Observations = od.Observations,
                    Status = od.Status
                }).ToList() ?? new List<OrderDetailDto>(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPatch("{id}/mark-as-paid")]
        public async Task<ActionResult<OrderResponseDto>> MarkOrderAsPaid(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Worker)
                .Include(o => o.PaymentMethod)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Pedido no encontrado" });
            }

            if (order.IsPaid)
            {
                return BadRequest(new { message = "El pedido ya está marcado como pagado" });
            }

            // ? Buscar caja activa
            var activeSmallBox = await _context.SmallBoxes
                .Where(sb => !sb.IsClosed)
                .OrderByDescending(sb => sb.OpeningDate)
                .FirstOrDefaultAsync();

            if (activeSmallBox == null)
            {
                return BadRequest(new { message = "No hay una caja activa. Debe abrir una caja antes de registrar pagos." });
            }

            // ? Marcar pedido como pagado
            order.IsPaid = true;
            order.Status = "Completada";
            order.CompletedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            // ? CREAR el movimiento de caja (Ingreso) AQUÍ
            var cashMovement = new CashMovement
            {
                MovementType = "Ingreso",
                Amount = order.Total,
                Concept = $"Pago de pedido {order.OrderNumber} - Mesa {order.Table?.NameTable ?? "Sin mesa"}",
                MovementDate = DateTime.UtcNow,
                SmallBoxId = activeSmallBox.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.CashMovements.Add(cashMovement);
            await _context.SaveChangesAsync();

            var response = new OrderResponseDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                UserId = order.UserId,
                TableId = order.TableId,
                TableName = order.Table?.NameTable,
                WorkerId = order.WorkerId,
                WorkerName = order.Worker != null ? $"{order.Worker.NameWorker} {order.Worker.LastNameWorker}" : null,
                PaymentMethodId = order.PaymentMethodId,
                PaymentMethodName = order.PaymentMethod?.NamePaymentMethod,
                SubTotal = order.SubTotal,
                Discount = order.Discount,
                Tax = order.Tax,
                Total = order.Total,
                CustomerName = order.CustomerName,
                OrderType = order.OrderType,
                Observations = order.Observations,
                IsPaid = order.IsPaid,
                CompletedAt = order.CompletedAt,
                OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    SubTotal = od.SubTotal,
                    Total = od.Total,
                    Observations = od.Observations,
                    Status = od.Status
                }).ToList() ?? new List<OrderDetailDto>(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            return Ok(response);
        }

        private async Task<bool> OrderExists(int id)
        {
            return await _context.Orders.AnyAsync(e => e.Id == id);
        }
    }
}
