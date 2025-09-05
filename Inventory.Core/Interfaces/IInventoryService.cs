using Inventory.Core.Dtos;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces;

public interface IInventoryService
{
    Task<int> CreateRequisitionAsync(int branchId, int userId, List<RequisitionMaterialDto> materials);

    // ✅ Dos versiones: una con ID y otra con Entidad
    Task ApproveRequisitionAsync(int requisitionId, int userId);

    Task<int> CreatePurchaseOrderFromRequisitionAsync(int requisitionId, int supplierId, int userId);
    Task RegisterReceivalAsync(int purchaseOrderId, List<ReceivedMaterialDto> receivedMaterials, int userId);
    Task<int> CreateInvoiceAsync(int purchaseOrderId, string uuid, decimal amount, int userId);
}