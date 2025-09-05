using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly InventoryDbContext _context;

    public InventoryService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateRequisitionAsync(int branchId, int userId, List<RequisitionMaterialDto> materials)
    {
        var requisition = new Requisition
        {
            BranchId = branchId,
            StatusId = 1, // Borrador
            InsertDate = DateTime.UtcNow,
            UserId = userId
        };

        await _context.Requisitions.AddAsync(requisition);
        await _context.SaveChangesAsync();

        foreach (var material in materials)
        {
            var mmuId = await GetMaterialMeasurementUnitId(material.MaterialId, material.MeasurementUnitId);
            if (mmuId == 0)
            {
                throw new Exception($"No se encontró la unidad de medida para el material ID {material.MaterialId}");
            }

            var materialRequisition = new MaterialRequisition
            {
                RequisitionId = requisition.IdRequisitions,
                MaterialMeasurementUnitId = mmuId,
                Quantity = material.Quantity
            };
            await _context.MaterialRequisitions.AddAsync(materialRequisition);
        }

        await _context.SaveChangesAsync();
        return requisition.IdRequisitions;
    }

    public async Task ApproveRequisitionAsync(int requisition, int userId)
    {
        
        if (requisition == null)
        {
            throw new Exception($"Requisición no encontrada");
        }

        var requisitionEntity = await _context.Requisitions.FindAsync(requisition);
        requisitionEntity.StatusId = 2;
        await _context.SaveChangesAsync();
    }

    public async Task<int> CreatePurchaseOrderFromRequisitionAsync(int requisitionId, int supplierId, int userId)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.MaterialRequisitions)
            .ThenInclude(mr => mr.MaterialMeasurementUnit) // ← ¡ESTA LÍNEA FALTABA!
            .FirstOrDefaultAsync(r => r.IdRequisitions == requisitionId);

        if (requisition == null)
        {
            throw new Exception($"Requisición con ID {requisitionId} no encontrada");
        }

        // El resto de tu código permanece igual...
        var purchaseOrder = new PurchaseOrder
        {
            SuppliersId = supplierId,
            RequisitionId = requisitionId,
            BranchId = requisition.BranchId,
            StatusId = 6, // Pendiente
            InsertDate = DateTime.UtcNow,
            UserId = userId
        };

        await _context.PurchaseOrders.AddAsync(purchaseOrder);
        await _context.SaveChangesAsync();

        foreach (var materialReq in requisition.MaterialRequisitions)
        {
            var materialSupplier = await _context.MaterialSuppliers
                .FirstOrDefaultAsync(ms => ms.MaterialId == materialReq.MaterialMeasurementUnit.MaterialId &&
                                         ms.SupplierId == supplierId);

            if (materialSupplier != null)
            {
                var materialPurchaseOrder = new MaterialPurchaseOrder
                {
                    PurchaseOrderId = purchaseOrder.IdPurchaseOrders,
                    MaterialSupplierId = materialSupplier.IdMaterialsSupplier,
                    MaterialMeasurementUnitId = materialReq.MaterialMeasurementUnitId,
                    Quantity = materialReq.Quantity,
                    QuantityIn = 0,
                    Cost = materialSupplier.Cost
                };

                await _context.MaterialPurchaseOrders.AddAsync(materialPurchaseOrder);
            }
        }

        requisition.StatusId = 4; 
        await _context.SaveChangesAsync();

        return purchaseOrder.IdPurchaseOrders;
    }

    public async Task RegisterReceivalAsync(int purchaseOrderId, List<ReceivedMaterialDto> receivedMaterials, int userId)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.MaterialPurchaseOrders)
            .FirstOrDefaultAsync(po => po.IdPurchaseOrders == purchaseOrderId);

        if (purchaseOrder == null)
        {
            throw new Exception($"Orden de compra con ID {purchaseOrderId} no encontrada");
        }

        // OBTENER EL ID CORRECTO dinámicamente
        var entradaMovement = await _context.TypeMovementInventories
            .FirstOrDefaultAsync(t => t.Name == "Entrada");

        if (entradaMovement == null)
        {
            throw new Exception("No se encontró el tipo de movimiento 'Entrada' en el sistema");
        }

        foreach (var receivedMaterial in receivedMaterials)
        {
            var materialPO = await _context.MaterialPurchaseOrders
                .FirstOrDefaultAsync(mpo => mpo.IdMaterialsPurchaseOrders == receivedMaterial.MaterialPurchaseOrderId);

            if (materialPO != null)
            {
                materialPO.QuantityIn += receivedMaterial.QuantityReceived;

                // Registrar movimiento de inventario - USAR ID DINÁMICO
                var movement = new MovementInventory
                {
                    TypeMovementsInventoryId = entradaMovement.IdTypeMovementsInventory, // ← ID CORRECTO
                    BranchId = purchaseOrder.BranchId,
                    MaterialSupplierId = materialPO.MaterialSupplierId,
                    MaterialMeasurementUnitId = materialPO.MaterialMeasurementUnitId,
                    Quantity = receivedMaterial.QuantityReceived,
                    InsertDate = DateTime.UtcNow,
                    UserId = userId
                };

                await _context.MovementInventories.AddAsync(movement);

                // Actualizar inventario por sucursal
                await UpdateInventoryByBranch(purchaseOrder.BranchId, materialPO.MaterialSupplierId,
                    materialPO.MaterialMeasurementUnitId, receivedMaterial.QuantityReceived,
                    materialPO.Cost ?? 0);
            }
        }

        // Verificar si se recibió todo
        var allReceived = purchaseOrder.MaterialPurchaseOrders.All(mpo => mpo.QuantityIn >= mpo.Quantity);
        purchaseOrder.StatusId = allReceived ? 10 : 9; // 10 = Recibida Completamente, 9 = Recibida Parcialmente

        await _context.SaveChangesAsync();
    }

    public async Task<int> CreateInvoiceAsync(int purchaseOrderId, string uuid, decimal amount, int userId)
    {
        var purchaseOrder = await _context.PurchaseOrders.FindAsync(purchaseOrderId);
        if (purchaseOrder == null)
        {
            throw new Exception($"Orden de compra con ID {purchaseOrderId} no encontrada");
        }

        var invoice = new InvoicePurchaseOrder
        {
            PurchaseOrderId = purchaseOrderId,
            UUID = uuid,
            Amount = amount,
            InsertDate = DateTime.UtcNow
        };

        await _context.InvoicePurchaseOrders.AddAsync(invoice);

        // Cambiar estado de la orden de compra a "Facturada"
        purchaseOrder.StatusId = 11; // Facturada
        await _context.SaveChangesAsync();

        return invoice.IdInvoicePurchaseOrders;
    }

    private async Task<int> GetMaterialMeasurementUnitId(int materialId, int measurementUnitId)
    {
        var mmu = await _context.MaterialsMeasurementUnits
            .FirstOrDefaultAsync(m => m.MaterialId == materialId && m.MeasurementUnitId == measurementUnitId);
        return mmu?.IdMaterialsMeasurementUnits ?? 0;
    }

    private async Task UpdateInventoryByBranch(int branchId, int materialSupplierId, int materialMeasurementUnitId, decimal quantity, decimal cost)
    {
        var inventory = await _context.InventoryByBranches
            .FirstOrDefaultAsync(ib => ib.BranchId == branchId &&
                                     ib.MaterialSupplierId == materialSupplierId &&
                                     ib.MaterialMeasurementUnitId == materialMeasurementUnitId);

        if (inventory == null)
        {
            inventory = new InventoryByBranch
            {
                BranchId = branchId,
                MaterialSupplierId = materialSupplierId,
                MaterialMeasurementUnitId = materialMeasurementUnitId,
                Quantity = quantity,
                Cost = cost
            };
            await _context.InventoryByBranches.AddAsync(inventory);
        }
        else
        {
            inventory.Quantity += quantity;
            // Actualizar costo promedio si es necesario
            inventory.Cost = (inventory.Cost + cost) / 2;
        }

        await _context.SaveChangesAsync();
    }
}