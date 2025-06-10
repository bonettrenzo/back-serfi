
namespace backend_serfi.Controllers;

using backend_serfi.Model;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private readonly Services.IUserService _userService;

    public UserController(Services.IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/User
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(long id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "Usuario no encontrado" });
        }

        return Ok(user);
    }

    // POST: api/User
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        try
        {
            var existingUser = await _userService.GetUserByEmailAsync(createUserDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "El email ya está registrado" });
            }

            var user = new User
            {
                NombreCompleto = createUserDto.NombreCompleto,
                Email = createUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                Pais = createUserDto.Pais,
                RolesId = createUserDto.RolesId,
                UltimaConexion = null
            };

            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(long id, UpdateUserDto updateUserDto)
    {
        try
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            existingUser.NombreCompleto = updateUserDto.NombreCompleto ?? existingUser.NombreCompleto;
            existingUser.Email = updateUserDto.Email ?? existingUser.Email;
            existingUser.Pais = updateUserDto.Pais ?? existingUser.Pais;
            existingUser.RolesId = updateUserDto.RolesId ?? existingUser.RolesId;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            await _userService.UpdateUserAsync(existingUser);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    // DELETE: api/User/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    // POST: api/User/login
    [HttpPost("login")]
    public async Task<ActionResult<UserWithRoleAndPermissionsDto>> Login(LoginDto loginDto)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return Unauthorized(new { message = "Email o contraseña incorrectos" });
            }

            user.UltimaConexion = DateTime.UtcNow;
            await _userService.UpdateUserAsync(user);


            var userWithRole = await _userService.UserWithRoleAndPermissions(user.Id);
             

            return Ok(userWithRole);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }


    // POST: api/User/change-password
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(changePasswordDto.UserId);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.Password))
            {
                return BadRequest(new { message = "Contraseña actual incorrecta" });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _userService.UpdateUserAsync(user);

            return Ok(new { message = "Contraseña actualizada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    [HttpGet("userWithRole/{id}")]
    public async Task<IActionResult> UserWithRoleAndPermissions(long id)
    {
        try
        {
            var user = await _userService.UserWithRoleAndPermissions(id);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(user);

        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });

        }
    }
}

public class CreateUserDto
{
    public required string NombreCompleto { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Pais { get; set; }
    public long RolesId { get; set; }
}

public class UpdateUserDto
{
    public string? NombreCompleto { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Pais { get; set; }
    public long? RolesId { get; set; }
}

public class UserDto
{
    public long Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public String Password { get; set; } = string.Empty;
    public DateTime? UltimaConexion { get; set; }
    public long RolesId { get; set; }
}

public class LoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginResponseDto
{
    public long Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public long RolesId { get; set; }
    public DateTime? UltimaConexion { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ChangePasswordDto
{
    public long UserId { get; set; }
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}

public class UserWithRoleAndPermissionsDto
{
    public long Id { get; set; }
    public string NombreCompleto { get; set; }
    public string Email { get; set; }
    public string Pais { get; set; }
    public string RolNombre { get; set; }
    public DateTime? UltimaConexion { get; set; }
    public List<string> Permisos { get; set; }
}

