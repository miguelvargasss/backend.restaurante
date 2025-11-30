using Backend.Restaurant.Data;
using Backend.Restaurant.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace Backend.Restaurant.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppData _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppData context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (loginDto == null) return BadRequest("Datos inválidos.");

            var user = await _context.Users
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Credenciales incorrectas (Usuario no encontrado)." });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new { message = "El usuario está desactivado. Contacte al administrador." });
            }

            try
            {
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Credenciales incorrectas (Contraseña errónea)." });
                }
            }
            catch (BCrypt.Net.SaltParseException ex)
            {
                return StatusCode(500, new 
                { 
                    message = "Error en el formato de la contraseña almacenada. Contacte al administrador.",
                    detail = "La contraseña debe ser regenerada con BCrypt"
                });
            }

            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var tokenString = GenerateJwtToken(user);

            return Ok(new
            {
                token = tokenString,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.NameUser,
                    firstName = user.NameUser,
                    role = user.Profil?.NameProfil ?? "Sin Rol"
                }
            });
        }

        private string GenerateJwtToken(Models.User user)
        {
            var key = Encoding.ASCII.GetBytes(
                _configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("JWT Key no configurada en appsettings.json")
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.NameUser),
                    new Claim(ClaimTypes.Role, user.Profil?.NameProfil ?? "User")
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout exitoso" });
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> Refresh()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _context.Users
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null || !user.IsActive)
                return Unauthorized();

            var tokenString = GenerateJwtToken(user);
            return Ok(new { token = tokenString });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var user = await _context.Users
                .Include(u => u.Profil)
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null)
                return NotFound();

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                name = user.NameUser,
                firstName = user.NameUser,
                role = user.Profil?.NameProfil ?? "Sin Rol"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest(new { message = "El email ya está registrado" });

            var profil = await _context.Profils.FindAsync(registerDto.ProfilId);
            if (profil == null)
                return BadRequest(new { message = "El perfil especificado no existe" });

            var user = new Models.User
            {
                NameUser = registerDto.Name,
                LastNameUser = registerDto.LastName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                IsActive = registerDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                ProfilId = registerDto.ProfilId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                message = "Usuario creado exitosamente", 
                userId = user.Id,
                email = user.Email,
                name = user.NameUser,
                lastName = user.LastNameUser,
                isActive = user.IsActive
            });
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                message = "Contraseña actualizada correctamente",
                email = user.Email,
                hashPreview = user.PasswordHash.Substring(0, 29) + "..."
            });
        }
    }
}
