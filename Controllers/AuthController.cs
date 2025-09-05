using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography; 
using System.Text;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly IUserService _userService; 

    public AuthController(IUserRepository userRepository, ITokenService tokenService, IPasswordService passwordService, IUserService userService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            // Validar si el usuario ya existe
            if (await _userRepository.UserExists(registerDto.Username, registerDto.Email))
            {
                return BadRequest("El usuario o email ya existe");
            }

            // Crear hash de la contraseña (sin salt)
            _passwordService.CreatePasswordHash(registerDto.Password, out byte[] passwordHash);

            // Crear nuevo usuario
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            return Ok(new { message = "Usuario registrado exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            Console.WriteLine("=== DEBUG LOGIN ===");
            Console.WriteLine($"Username: {loginDto.Username}");
            Console.WriteLine($"Password: {loginDto.Password}");

            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);

            if (user == null)
            {
                Console.WriteLine("Usuario no encontrado");
                return Unauthorized("Credenciales inválidas");
            }

            Console.WriteLine($"User ID: {user.IdUsers}");
            Console.WriteLine($"Stored hash length: {user.PasswordHash?.Length}");

            if (user.PasswordHash == null || user.PasswordHash.Length == 0)
            {
                Console.WriteLine("PasswordHash es NULL o vacío");
                return Unauthorized("Credenciales inválidas");
            }

            Console.WriteLine($"Stored hash: {BitConverter.ToString(user.PasswordHash).Replace("-", "")}");

            // Generar hash para comparar
            using var sha256 = SHA256.Create();
            var computedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            Console.WriteLine($"Computed hash: {BitConverter.ToString(computedHash).Replace("-", "")}");

            bool isMatch = computedHash.SequenceEqual(user.PasswordHash);
            Console.WriteLine($"Hash match: {isMatch}");

            if (!isMatch)
            {
                Console.WriteLine("Los hashes NO coinciden");
                return Unauthorized("Credenciales inválidas");
            }

            if (!user.IsActive)
            {
                Console.WriteLine("Usuario inactivo");
                return Unauthorized("Usuario inactivo");
            }

            Console.WriteLine("Login exitoso");
            var token = _tokenService.CreateToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.IdUsers,
                    user.Username,
                    user.Email,
                    roles = user.UserRoles.Select(ur => ur.Role.Code)
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Verificar contraseña actual
            using var sha256 = SHA256.Create();
            var currentHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(changePasswordDto.CurrentPassword));

            if (!currentHash.SequenceEqual(user.PasswordHash))
            {
                return BadRequest("Contraseña actual incorrecta");
            }

            // Crear nueva contraseña
            _passwordService.CreatePasswordHash(changePasswordDto.NewPassword, out byte[] newPasswordHash);

            user.PasswordHash = newPasswordHash;
            await _userRepository.UpdateAsync(user);

            return Ok(new { message = "Contraseña cambiada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound("Usuario no encontrado");
            }

            return Ok(new
            {
                user.IdUsers,
                user.Username,
                user.Email,
                user.IsActive,
                user.CreatedAt,
                roles = user.UserRoles.Select(ur => ur.Role.Code)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    // Método de utilidad para debug (puedes removerlo después)
    [HttpPost("debug-hash")]
    [AllowAnonymous]
    public IActionResult GenerateHash([FromBody] DebugHashRequest request)
    {
        try
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

            return Ok(new
            {
                password = request.Password,
                hashHex = "0x" + BitConverter.ToString(hash).Replace("-", ""),
                hashLength = hash.Length
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpPost("assign-role")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleDto assignRoleDto)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _userService.AssignRoleToUserAsync(
                assignRoleDto.Username,
                assignRoleDto.RoleCode,
                currentUserId);

            return Ok(new { message = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al asignar rol: {ex.Message}");
        }
    }

    [HttpPost("remove-role")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] RemoveRoleDto removeRoleDto)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var result = await _userService.RemoveRoleFromUserAsync(
                removeRoleDto.Username,
                removeRoleDto.RoleCode,
                currentUserId);

            return Ok(new { message = result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al remover rol: {ex.Message}");
        }
    }

    [HttpGet("user-roles/{username}")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetUserRoles(string username)
    {
        try
        {
            var roles = await _userService.GetUserRolesAsync(username);
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener roles: {ex.Message}");
        }
    }

    [HttpGet("all-users-with-roles")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> GetAllUsersWithRoles()
    {
        try
        {
            var users = await _userService.GetAllUsersWithRolesAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener usuarios: {ex.Message}");
        }
    }
}

// DTOs
public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class DebugHashRequest
{
    public string Password { get; set; } = string.Empty;
}

public class AssignRoleDto
{
    public string Username { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
}

public class RemoveRoleDto
{
    public string Username { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
}