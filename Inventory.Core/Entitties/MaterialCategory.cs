using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Entities;

public class MaterialCategory
{
    public int IdMaterialCategory { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}