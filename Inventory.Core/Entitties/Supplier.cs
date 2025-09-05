using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Inventory.Core.Entities;

public class Supplier
{
    public int IdSuppliers { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public string TaxName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string NameContact { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
