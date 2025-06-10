using backend_serfi.Context;
using backend_serfi.Controllers;
using backend_serfi.Model;
using Microsoft.EntityFrameworkCore;

namespace backend_serfi.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(long id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<UserDto> CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(long id);

    Task <UserWithRoleAndPermissionsDto> UserWithRoleAndPermissions(long id);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            NombreCompleto = u.NombreCompleto,
            Email = u.Email,
            Pais = u.Pais,
            RolesId = u.RolesId,
            UltimaConexion = u.UltimaConexion
        });
    }

    public async Task<UserWithRoleAndPermissionsDto?> UserWithRoleAndPermissions(long id)
    {
        return await _userRepository.UserWithRoleAndPermissions(id);
    }

    public async Task<User?> GetUserByIdAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return null;

        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<UserDto> CreateUserAsync(User user)
    {
        var createdUser = await _userRepository.CreateAsync(user);
        return new UserDto
        {
            Id = createdUser.Id,
            NombreCompleto = createdUser.NombreCompleto,
            Email = createdUser.Email,
            Pais = createdUser.Pais,
            RolesId = createdUser.RolesId,
            UltimaConexion = createdUser.UltimaConexion
        };
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(long id)
    {
        await _userRepository.DeleteAsync(id);
    }
}

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(long id);
    Task <UserWithRoleAndPermissionsDto> UserWithRoleAndPermissions(long id);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Roles)
            .ToListAsync();
    }

    public async Task<UserWithRoleAndPermissionsDto> UserWithRoleAndPermissions (long id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserWithRoleAndPermissionsDto
            {
                Id = u.Id,
                NombreCompleto = u.NombreCompleto,
                Email = u.Email,
                Pais = u.Pais,
                RolNombre = u.Roles.Nombre,
                UltimaConexion = u.UltimaConexion,
                Permisos = u.Roles.RolPermisos
                    .Select(rp => rp.Permiso.NombrePermiso)
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}