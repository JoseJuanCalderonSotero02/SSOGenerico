using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly InventoryDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserService(InventoryDbContext context, IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _context = context;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<string> AssignRoleToUserAsync(string username, string roleCode, int assignedByUserId)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            throw new Exception($"Usuario '{username}' no encontrado");

        var role = await _roleRepository.GetByCodeAsync(roleCode);
        if (role == null)
            throw new Exception($"Rol '{roleCode}' no encontrado");

        var existingUserRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.IdUsers == user.IdUsers && ur.IdRoles == role.IdRoles);

        if (existingUserRole != null)
            return $"El usuario '{username}' ya tiene el rol '{roleCode}'";

        var userRole = new UserRole
        {
            IdUsers = user.IdUsers,
            IdRoles = role.IdRoles,
            AssignedDate = DateTime.UtcNow,
            AssignedByIdUsers = assignedByUserId
        };

        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();

        return $"Rol '{roleCode}' asignado exitosamente a '{username}'";
    }

    public async Task<string> RemoveRoleFromUserAsync(string username, string roleCode, int removedByUserId)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            throw new Exception($"Usuario '{username}' no encontrado");

        var role = await _roleRepository.GetByCodeAsync(roleCode);
        if (role == null)
            throw new Exception($"Rol '{roleCode}' no encontrado");

        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.IdUsers == user.IdUsers && ur.IdRoles == role.IdRoles);

        if (userRole == null)
            return $"El usuario '{username}' no tiene el rol '{roleCode}'";

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return $"Rol '{roleCode}' removido exitosamente de '{username}'";
    }

    public async Task<List<string>> GetUserRolesAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            throw new Exception($"Usuario '{username}' no encontrado");

        return user.UserRoles.Select(ur => ur.Role.Code).ToList();
    }

    public async Task<List<UserWithRolesDto>> GetAllUsersWithRolesAsync()
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Select(u => new UserWithRolesDto
            {
                IdUsers = u.IdUsers,
                Username = u.Username,
                Email = u.Email,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Roles = u.UserRoles.Select(ur => ur.Role.Code).ToList()
            })
            .ToListAsync();

        return users;
    }
}