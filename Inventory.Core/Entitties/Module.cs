using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Core.Entities
{
    [Table("Modules", Schema = "sso")]
    public class Module
    {
        public int IdModule { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsActive { get; set; } = true;
        public int? ParentModuleId { get; set; }

        // Navigation properties
        public virtual Module? ParentModule { get; set; }
        public virtual ICollection<Module> ChildModules { get; set; } = new HashSet<Module>();
        public virtual ICollection<Permission> Permissions { get; set; } = new HashSet<Permission>();
    }
}