using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Entities;

public class MovementInventory
{
    public int IdMovementsInventory { get; set; }
    public int TypeMovementsInventoryId { get; set; }
    public int BranchId { get; set; }
    public int MaterialSupplierId { get; set; }
    public int MaterialMeasurementUnitId { get; set; }
    public decimal Quantity { get; set; }
    public DateTime InsertDate { get; set; } = DateTime.UtcNow;
    public int? UserId { get; set; }

    public virtual TypeMovementInventory TypeMovementInventory { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual MaterialSupplier MaterialSupplier { get; set; } = null!;
    public virtual MaterialsMeasurementUnits MaterialMeasurementUnit { get; set; } = null!;
    public virtual User? User { get; set; }
}
