using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder> GetByIdAsync(int id);
    Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(int statusId);
    Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(int supplierId);
    Task AddAsync(PurchaseOrder purchaseOrder);
    Task UpdateAsync(PurchaseOrder purchaseOrder);
}