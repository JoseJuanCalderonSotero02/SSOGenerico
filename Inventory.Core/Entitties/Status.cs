using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Entities;

public class Status
{
    public int IdStatuses { get; set; }
    public string Entity { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}