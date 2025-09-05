using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Entities;

public class InventoryByBranch
{
    public int IdInventoryByBranch { get; set; }
    public int BranchId { get; set; }
    public int MaterialSupplierId { get; set; }
    public int MaterialMeasurementUnitId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Cost { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual MaterialSupplier MaterialSupplier { get; set; } = null!;
    public virtual MaterialsMeasurementUnits MaterialMeasurementUnit { get; set; } = null!;
}