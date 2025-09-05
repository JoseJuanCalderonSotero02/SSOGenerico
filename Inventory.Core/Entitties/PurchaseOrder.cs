using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Entities;

public class PurchaseOrder
{
    public int IdPurchaseOrders { get; set; }
    public int SuppliersId { get; set; }
    public int RequisitionId { get; set; }
    public int BranchId { get; set; }
    public int StatusId { get; set; }
    public DateTime InsertDate { get; set; } = DateTime.UtcNow;
    public int? UserId { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
    public virtual Requisition Requisition { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual Status Status { get; set; } = null!;
    public virtual User? User { get; set; }
    public virtual ICollection<MaterialPurchaseOrder> MaterialPurchaseOrders { get; set; } = new List<MaterialPurchaseOrder>();
}