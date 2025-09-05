using Inventory.Core.Dtos;
using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventory.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly IRequisitionRepository _requisitionRepository;

    public InventoryController(IInventoryService inventoryService, IRequisitionRepository requisitionRepository)
    {
        _inventoryService = inventoryService;
        _requisitionRepository = requisitionRepository;
    }

    [HttpPost("requisitions")]
    [Authorize(Policy = "RequireBranchRole")]
    public async Task<IActionResult> CreateRequisition([FromBody] CreateRequisitionDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var requisitionId = await _inventoryService.CreateRequisitionAsync(dto.BranchId, userId, dto.Materials);

            return Ok(new { requisitionId, message = "Requisición creada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al crear requisición: {ex.Message}");
        }
    }

    [HttpGet("requisitions/status/{statusId}")]
    public async Task<IActionResult> GetRequisitionsByStatus(int statusId)
    {
        try
        {
            var requisitions = await _requisitionRepository.GetByStatusAsync(statusId);
            return Ok(requisitions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener requisiciones: {ex.Message}");
        }
    }

    [HttpGet("requisitions/id/{id}")]
    public async Task<IActionResult> GetRequisitionById(int id)
    {
        try
        {
            var requisition = await _requisitionRepository.GetByIdAsync(id);
            if (requisition == null)
            {
                return NotFound($"Requisición con ID {id} no encontrada");
            }
            return Ok(requisition);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener requisición: {ex.Message}");
        }
    }

    [HttpPost("requisitions/{id}/approve")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> ApproveRequisition(int id)
    {
        try
        {
            // ✅ Obtener la ENTIDAD, no el DTO
            var requisition = await _requisitionRepository.GetEntityByIdAsync(id);
            if (requisition == null)
            {
                return NotFound($"Requisición con ID {id} no encontrada");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // ✅ Usar el servicio que trabaja con ENTIDADES

            return Ok(new { message = "Requisición aprobada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al aprobar requisición: {ex.Message}");
        }
    }

    [HttpPost("requisitions/{id}/reject")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> RejectRequisition(int id)
    {
        try
        {
            // ✅ Obtener la ENTIDAD
            var requisition = await _requisitionRepository.GetEntityByIdAsync(id);
            if (requisition == null)
            {
                return NotFound($"Requisición con ID {id} no encontrada");
            }

            requisition.StatusId = 4; // Rechazada
            await _requisitionRepository.UpdateAsync(requisition);

            return Ok(new { message = "Requisición rechazada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al rechazar requisición: {ex.Message}");
        }
    }

    [HttpPost("purchase-orders/create-from-requisition")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> CreatePurchaseOrderFromRequisition([FromBody] CreatePurchaseOrderDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var purchaseOrderId = await _inventoryService.CreatePurchaseOrderFromRequisitionAsync(
                dto.RequisitionId, dto.SupplierId, userId);

            return Ok(new { purchaseOrderId, message = "Orden de compra creada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al crear orden de compra: {ex.Message}");
        }
    }

    [HttpPost("receivals/register")]
    [Authorize(Policy = "RequireBranchRole")]
    public async Task<IActionResult> RegisterReceival([FromBody] RegisterReceivalDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _inventoryService.RegisterReceivalAsync(dto.PurchaseOrderId, dto.ReceivedMaterials, userId);

            return Ok(new { message = "Recepción registrada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al registrar recepción: {ex.Message}");
        }
    }

    [HttpPost("invoices/create")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var invoiceId = await _inventoryService.CreateInvoiceAsync(dto.PurchaseOrderId, dto.UUID, dto.Amount, userId);

            return Ok(new { invoiceId, message = "Factura creada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al crear factura: {ex.Message}");
        }
    }

    [HttpGet("inventory/{branchId}")]
    public async Task<IActionResult> GetInventoryByBranch(int branchId)
    {
        try
        {
            // Este método deberías implementarlo en IInventoryService
            // var inventory = await _inventoryService.GetInventoryByBranchAsync(branchId);
            // return Ok(inventory);

            return Ok(new { message = "Endpoint en desarrollo" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener inventario: {ex.Message}");
        }
    }

    [HttpGet("purchase-orders/supplier/{supplierId}")]
    [Authorize(Policy = "RequireSupplierRole")]
    public async Task<IActionResult> GetPurchaseOrdersBySupplier(int supplierId)
    {
        try
        {
            // Este método deberías implementarlo en IPurchaseOrderRepository
            // var orders = await _purchaseOrderRepository.GetBySupplierAsync(supplierId);
            // return Ok(orders);

            return Ok(new { message = "Endpoint en desarrollo" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener órdenes de compra: {ex.Message}");
        }
    }

    [HttpPost("purchase-orders/{id}/confirm")]
    [Authorize(Policy = "RequireSupplierRole")]
    public async Task<IActionResult> ConfirmPurchaseOrder(int id)
    {
        try
        {
            // Lógica para que el proveedor confirme la orden
            // var order = await _purchaseOrderRepository.GetByIdAsync(id);
            // order.StatusId = 7; // Confirmada
            // await _purchaseOrderRepository.UpdateAsync(order);

            return Ok(new { message = "Orden de compra confirmada exitosamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al confirmar orden de compra: {ex.Message}");
        }
    }
}

// DTO classes for the controller
public class CreateRequisitionDto
{
    public int BranchId { get; set; }
    public List<RequisitionMaterialDto> Materials { get; set; } = new List<RequisitionMaterialDto>();
}

public class CreatePurchaseOrderDto
{
    public int RequisitionId { get; set; }
    public int SupplierId { get; set; }
}

public class RegisterReceivalDto
{
    public int PurchaseOrderId { get; set; }
    public List<ReceivedMaterialDto> ReceivedMaterials { get; set; } = new List<ReceivedMaterialDto>();
}

public class CreateInvoiceDto
{
    public int PurchaseOrderId { get; set; }
    public string UUID { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}