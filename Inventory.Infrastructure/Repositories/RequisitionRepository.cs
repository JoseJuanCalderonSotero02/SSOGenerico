using Inventory.Core.Dtos;
using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class RequisitionRepository : IRequisitionRepository
{
    private readonly InventoryDbContext _context;

    public RequisitionRepository(InventoryDbContext context)
    {
        _context = context;
    }

    // ✅ MÉTODOS DE LECTURA (devuelven DTOs)
    public async Task<RequisitionDto> GetByIdAsync(int id)
    {
        return await _context.Requisitions
            .Where(r => r.IdRequisitions == id)
            .Select(r => new RequisitionDto
            {
                IdRequisitions = r.IdRequisitions,
                BranchId = r.BranchId,
                BranchName = r.Branch.Name,
                StatusId = r.StatusId,
                StatusName = r.Status.Name,
                InsertDate = r.InsertDate,
                UserId = r.UserId,
                Username = r.User != null ? r.User.Username : null,
                Materials = r.MaterialRequisitions.Select(mr => new MaterialRequisitionDto
                {
                    IdMaterialRequisition = mr.IdMaterialRequisition,
                    MaterialId = mr.MaterialMeasurementUnit.MaterialId,
                    MaterialName = mr.MaterialMeasurementUnit.Material.Name,
                    MaterialShortName = mr.MaterialMeasurementUnit.Material.ShortName,
                    MeasurementUnitId = mr.MaterialMeasurementUnit.MeasurementUnitId,
                    UnitName = mr.MaterialMeasurementUnit.MeasurementUnit.Name,
                    UnitShortName = mr.MaterialMeasurementUnit.MeasurementUnit.ShortName,
                    Quantity = mr.Quantity
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<RequisitionSummaryDto>> GetByStatusAsync(int statusId)
    {
        return await _context.Requisitions
            .Where(r => r.StatusId == statusId)
            .Select(r => new RequisitionSummaryDto
            {
                IdRequisitions = r.IdRequisitions,
                BranchName = r.Branch.Name,
                StatusName = r.Status.Name,
                InsertDate = r.InsertDate,
                Username = r.User != null ? r.User.Username : null,
                MaterialCount = r.MaterialRequisitions.Count,
                TotalQuantity = r.MaterialRequisitions.Sum(mr => mr.Quantity)
            })
            .ToListAsync();
    }

    // ✅ MÉTODO PARA OBTENER LA ENTIDAD (para operaciones de escritura)
    public async Task<Requisition> GetEntityByIdAsync(int id)
    {
        return await _context.Requisitions
            .Include(r => r.MaterialRequisitions)
            .FirstOrDefaultAsync(r => r.IdRequisitions == id);
    }

    // ✅ MÉTODOS DE ESCRITURA (trabajan con ENTIDADES)
    public async Task AddAsync(Requisition requisition)
    {
        await _context.Requisitions.AddAsync(requisition);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Requisition requisition)
    {
        _context.Requisitions.Update(requisition);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var requisition = await GetEntityByIdAsync(id);
        if (requisition != null)
        {
            _context.Requisitions.Remove(requisition);
            await _context.SaveChangesAsync();
        }
    }
}