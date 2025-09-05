using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Dtos;

public class RequisitionMaterialDto
{
    public int MaterialId { get; set; }
    public int MeasurementUnitId { get; set; }
    public decimal Quantity { get; set; }
}