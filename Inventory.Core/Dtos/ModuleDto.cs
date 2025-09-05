using System.Collections.Generic;

namespace Inventory.Core.Dtos
{
    public class ModuleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<ModuleDto> Children { get; set; } = new List<ModuleDto>();
    }
}