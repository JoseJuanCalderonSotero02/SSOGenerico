using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Core.Entities;

public class User
{
    public int IdUsers { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [Column(TypeName = "varbinary(30)")]
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public class Role
{
    public int IdRoles { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public class UserRole
{
    public int IdUsers { get; set; }
    public int IdRoles { get; set; }
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    public int? AssignedByIdUsers { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
    public virtual User? AssignedBy { get; set; }
}
