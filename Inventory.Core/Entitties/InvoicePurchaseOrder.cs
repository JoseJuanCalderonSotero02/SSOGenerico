using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Inventory.Core.Entities;

public class InvoicePurchaseOrder
{
    public int IdInvoicePurchaseOrders { get; set; }
    public int PurchaseOrderId { get; set; }
    public string UUID { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime InsertDate { get; set; } = DateTime.UtcNow;

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
}