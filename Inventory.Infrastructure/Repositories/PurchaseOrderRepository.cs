using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly InventoryDbContext _context;

    public PurchaseOrderRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder> GetByIdAsync(int id)
    {
        return await _context.PurchaseOrders
            .Include(po => po.MaterialPurchaseOrders)
            .FirstOrDefaultAsync(po => po.IdPurchaseOrders == id);
    }

    public async Task<IEnumerable<PurchaseOrder>> GetByStatusAsync(int statusId)
    {
        return await _context.PurchaseOrders
            .Include(po => po.MaterialPurchaseOrders)
            .Where(po => po.StatusId == statusId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PurchaseOrder>> GetBySupplierAsync(int supplierId)
    {
        return await _context.PurchaseOrders
            .Include(po => po.MaterialPurchaseOrders)
            .Where(po => po.SuppliersId == supplierId)
            .ToListAsync();
    }

    public async Task AddAsync(PurchaseOrder purchaseOrder)
    {
        await _context.PurchaseOrders.AddAsync(purchaseOrder);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PurchaseOrder purchaseOrder)
    {
        _context.PurchaseOrders.Update(purchaseOrder);
        await _context.SaveChangesAsync();
    }
}