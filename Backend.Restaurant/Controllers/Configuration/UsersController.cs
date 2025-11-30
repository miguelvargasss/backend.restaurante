using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Configuration.Users;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Restaurant.Controllers.Configuration
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppData _context;

        public UsersController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedUsersResponseDto>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? profilId = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Users
                .Include(u => u.Profil)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u => 
                    u.NameUser.ToLower().Contains(search) ||
                    u.LastNameUser.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            if (profilId.HasValue)
            {
                query = query.Where(u => u.ProfilId == profilId.Value);
            }

            var total = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.NameUser,
                    LastName = u.LastNameUser,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    ProfilId = u.ProfilId,
                    ProfilName = u.Profil != null ? u.Profil.NameProfil : null,
                    LastLogin = u.LastLogin,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            var response = new PaginatedUsersResponseDto
            {
                Users = users,
                Total = total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            var response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.NameUser,
                LastName = user.LastNameUser,
                Email = user.Email,
                IsActive = user.IsActive,
                ProfilId = user.ProfilId,
                ProfilName = user.Profil?.NameProfil,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email))
            {
                return BadRequest(new { message = "El email ya está registrado" });
            }

            var profil = await _context.Profils.FindAsync(createUserDto.ProfilId);
            if (profil == null)
            {
                return BadRequest(new { message = "El perfil especificado no existe" });
            }

            var user = new User
            {
                NameUser = createUserDto.Name,
                LastNameUser = createUserDto.LastName,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                IsActive = createUserDto.IsActive,
                ProfilId = createUserDto.ProfilId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _context.Entry(user).Reference(u => u.Profil).LoadAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.NameUser,
                LastName = user.LastNameUser,
                Email = user.Email,
                IsActive = user.IsActive,
                ProfilId = user.ProfilId,
                ProfilName = user.Profil?.NameProfil,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            if (await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email && u.Id != id))
            {
                return BadRequest(new { message = "El email ya está registrado en otro usuario" });
            }

            var profil = await _context.Profils.FindAsync(updateUserDto.ProfilId);
            if (profil == null)
            {
                return BadRequest(new { message = "El perfil especificado no existe" });
            }

            user.NameUser = updateUserDto.Name;
            user.LastNameUser = updateUserDto.LastName;
            user.Email = updateUserDto.Email;
            user.IsActive = updateUserDto.IsActive;
            user.ProfilId = updateUserDto.ProfilId;
            user.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id))
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }
                else
                {
                    throw;
                }
            }

            await _context.Entry(user).Reference(u => u.Profil).LoadAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.NameUser,
                LastName = user.LastNameUser,
                Email = user.Email,
                IsActive = user.IsActive,
                ProfilId = user.ProfilId,
                ProfilName = user.Profil?.NameProfil,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == id);
            var hasReserves = await _context.Reserves.AnyAsync(r => r.UserId == id);
            var hasSmallBoxes = await _context.SmallBoxes.AnyAsync(sb => sb.UserId == id);

            if (hasOrders || hasReserves || hasSmallBoxes)
            {
                return BadRequest(new 
                { 
                    message = "No se puede eliminar el usuario porque tiene registros asociados. " +
                              "Considere desactivar el usuario en su lugar."
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario eliminado exitosamente" });
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<UserResponseDto>> ToggleUserStatus(int id)
        {
            var user = await _context.Users
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.NameUser,
                LastName = user.LastNameUser,
                Email = user.Email,
                IsActive = user.IsActive,
                ProfilId = user.ProfilId,
                ProfilName = user.Profil?.NameProfil,
                LastLogin = user.LastLogin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(response);
        }

        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
    }
}
