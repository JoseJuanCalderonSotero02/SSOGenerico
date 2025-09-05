using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Core.Entities
{
    [Table("RolePermissions", Schema = "sso")]
    public class RolePermission
    {
        public int IdRolePermission { get; set; }

        // Foreign keys
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        // Navigation properties
        public virtual Role Role { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
    }
}