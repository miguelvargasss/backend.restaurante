using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Configuration.Profiles;
using Backend.Restaurant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Restaurant.Controllers.Configuration
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfilesController : ControllerBase
    {
        private readonly AppData _context;

        public ProfilesController(AppData context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedProfilesResponseDto>> GetProfiles(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Profils.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(p => 
                    p.NameProfil.ToLower().Contains(search) ||
                    p.DescriptionProfil.ToLower().Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            var total = await query.CountAsync();

            var profiles = await query
                .OrderBy(p => p.NameProfil)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProfileResponseDto
                {
                    Id = p.Id,
                    Name = p.NameProfil,
                    Description = p.DescriptionProfil,
                    CreatedAt = p.CreatedAt.ToString("yyyy-MM-dd"),
                    HasAdminAccess = p.HasAdminAccess,
                    IsActive = p.IsActive
                })
                .ToListAsync();

            var response = new PaginatedProfilesResponseDto
            {
                Profiles = profiles,
                Total = total,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("simple")]
        public async Task<IActionResult> GetSimpleProfiles()
        {
            var profiles = await _context.Profils
                .Where(p => p.IsActive)
                .OrderBy(p => p.NameProfil)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.NameProfil,
                    description = p.DescriptionProfil
                })
                .ToListAsync();

            return Ok(profiles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileResponseDto>> GetProfile(int id)
        {
            var profile = await _context.Profils.FindAsync(id);

            if (profile == null)
            {
                return NotFound(new { message = "Perfil no encontrado" });
            }

            var response = new ProfileResponseDto
            {
                Id = profile.Id,
                Name = profile.NameProfil,
                Description = profile.DescriptionProfil,
                CreatedAt = profile.CreatedAt.ToString("yyyy-MM-dd"),
                HasAdminAccess = profile.HasAdminAccess,
                IsActive = profile.IsActive
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ProfileResponseDto>> CreateProfile([FromBody] CreateProfileDto createProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Profils.AnyAsync(p => p.NameProfil == createProfileDto.Name))
            {
                return BadRequest(new { message = "Ya existe un perfil con ese nombre" });
            }

            var profile = new Profil
            {
                NameProfil = createProfileDto.Name,
                DescriptionProfil = createProfileDto.Description,
                HasAdminAccess = createProfileDto.HasAdminAccess,
                IsActive = createProfileDto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Profils.Add(profile);
            await _context.SaveChangesAsync();

            var response = new ProfileResponseDto
            {
                Id = profile.Id,
                Name = profile.NameProfil,
                Description = profile.DescriptionProfil,
                CreatedAt = profile.CreatedAt.ToString("yyyy-MM-dd"),
                HasAdminAccess = profile.HasAdminAccess,
                IsActive = profile.IsActive
            };

            return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProfileResponseDto>> UpdateProfile(int id, [FromBody] UpdateProfileDto updateProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var profile = await _context.Profils.FindAsync(id);

            if (profile == null)
            {
                return NotFound(new { message = "Perfil no encontrado" });
            }

            if (await _context.Profils.AnyAsync(p => p.NameProfil == updateProfileDto.Name && p.Id != id))
            {
                return BadRequest(new { message = "Ya existe otro perfil con ese nombre" });
            }

            profile.NameProfil = updateProfileDto.Name;
            profile.DescriptionProfil = updateProfileDto.Description;
            profile.HasAdminAccess = updateProfileDto.HasAdminAccess;
            profile.IsActive = updateProfileDto.IsActive;
            profile.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProfileExists(id))
                {
                    return NotFound(new { message = "Perfil no encontrado" });
                }
                else
                {
                    throw;
                }
            }

            var response = new ProfileResponseDto
            {
                Id = profile.Id,
                Name = profile.NameProfil,
                Description = profile.DescriptionProfil,
                CreatedAt = profile.CreatedAt.ToString("yyyy-MM-dd"),
                HasAdminAccess = profile.HasAdminAccess,
                IsActive = profile.IsActive
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var profile = await _context.Profils.FindAsync(id);

            if (profile == null)
            {
                return NotFound(new { message = "Perfil no encontrado" });
            }

            var hasUsers = await _context.Users.AnyAsync(u => u.ProfilId == id);

            if (hasUsers)
            {
                return BadRequest(new 
                { 
                    message = "No se puede eliminar el perfil porque tiene usuarios asignados. " +
                              "Considere desactivar el perfil en su lugar."
                });
            }

            _context.Profils.Remove(profile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Perfil eliminado exitosamente" });
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<ProfileResponseDto>> ToggleProfileStatus(int id)
        {
            var profile = await _context.Profils.FindAsync(id);

            if (profile == null)
            {
                return NotFound(new { message = "Perfil no encontrado" });
            }

            profile.IsActive = !profile.IsActive;
            profile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new ProfileResponseDto
            {
                Id = profile.Id,
                Name = profile.NameProfil,
                Description = profile.DescriptionProfil,
                CreatedAt = profile.CreatedAt.ToString("yyyy-MM-dd"),
                HasAdminAccess = profile.HasAdminAccess,
                IsActive = profile.IsActive
            };

            return Ok(response);
        }

        private async Task<bool> ProfileExists(int id)
        {
            return await _context.Profils.AnyAsync(e => e.Id == id);
        }
    }
}
