using Inventory.Core.Dtos;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces;

public interface IRequisitionRepository
{
    // Métodos de lectura (DTOs)
    Task<RequisitionDto> GetByIdAsync(int id);
    Task<List<RequisitionSummaryDto>> GetByStatusAsync(int statusId);

    // Método para obtener entidad (para escritura)
    Task<Requisition> GetEntityByIdAsync(int id);

    // Métodos de escritura (entidades)
    Task AddAsync(Requisition requisition);
    Task UpdateAsync(Requisition requisition);
    Task DeleteAsync(int id);
}