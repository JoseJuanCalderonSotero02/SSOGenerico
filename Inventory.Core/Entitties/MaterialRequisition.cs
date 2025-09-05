using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Inventory.Core.Entities;

public class MaterialRequisition
{
    public int IdMaterialRequisition { get; set; }
    public int RequisitionId { get; set; }
    public int MaterialMeasurementUnitId { get; set; }
    public decimal Quantity { get; set; }
    public virtual MaterialsMeasurementUnits MaterialMeasurementUnit { get; set; } = null!;

    public virtual Requisition Requisition { get; set; } = null!;
    
}
