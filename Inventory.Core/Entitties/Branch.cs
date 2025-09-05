using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace Inventory.Core.Entities;

public class Branch
{
    public int IdBranches { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IdCompany { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}