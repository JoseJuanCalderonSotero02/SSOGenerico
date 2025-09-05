using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Inventory.Core.Entities;

public class MaterialSupplier
{
    public int IdMaterialsSupplier { get; set; }
    public int MaterialId { get; set; }
    public int SupplierId { get; set; }
    public decimal? Cost { get; set; }
    public int? UserId { get; set; }

    public virtual Material Material { get; set; } = null!;
    public virtual Supplier Supplier { get; set; } = null!;
    public virtual User? User { get; set; }
}