using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces;

public interface IUserService
{
    Task<string> AssignRoleToUserAsync(string username, string roleCode, int assignedByUserId);
    Task<string> RemoveRoleFromUserAsync(string username, string roleCode, int removedByUserId);
    Task<List<string>> GetUserRolesAsync(string username);
    Task<List<UserWithRolesDto>> GetAllUsersWithRolesAsync();
}

public class UserWithRolesDto
{
    public int IdUsers { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}