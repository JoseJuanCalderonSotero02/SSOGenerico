using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Inventory.Core.Entities;

public class Requisition
{
    public int IdRequisitions { get; set; }
    public int BranchId { get; set; }
    public int StatusId { get; set; }
    public DateTime InsertDate { get; set; } = DateTime.UtcNow;
    public int? UserId { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual Status Status { get; set; } = null!;
    public virtual User? User { get; set; }
    public virtual ICollection<MaterialRequisition> MaterialRequisitions { get; set; } = new List<MaterialRequisition>();
}