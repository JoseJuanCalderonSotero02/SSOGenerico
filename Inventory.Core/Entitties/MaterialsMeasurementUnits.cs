using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Inventory.Core.Entities;

public class MaterialsMeasurementUnits
{
    public int IdMaterialsMeasurementUnits { get; set; }
    public int MaterialId { get; set; }
    public int MeasurementUnitId { get; set; }
    public bool IsActive { get; set; }
    public int? UserId { get; set; }

    public virtual Material Material { get; set; } = null!;
    public virtual MeasurementUnit MeasurementUnit { get; set; } = null!;
    public virtual User? User { get; set; }
}