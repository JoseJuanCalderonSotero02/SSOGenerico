using Inventory.Core.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inventory.Core.Interfaces
{
    public interface IModuleService
    {
        Task<IEnumerable<ModuleDto>> GetModulesByUserIdAsync(int userId);
        Task<bool> HasPermissionAsync(int userId, string permissionCode);
    }
}