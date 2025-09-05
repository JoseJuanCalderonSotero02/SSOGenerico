using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Core.Entities
{
    [Table("Permissions", Schema = "sso")]
    public class Permission
    {
        public int IdPermission { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Foreign keys
        public int ModuleId { get; set; }

        // Navigation properties
        public virtual Module Module { get; set; } = null!;
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
    }
}