using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Inventory.Core.Dtos;

public class ReceivedMaterialDto
{
    public int MaterialPurchaseOrderId { get; set; }
    public decimal QuantityReceived { get; set; }
}