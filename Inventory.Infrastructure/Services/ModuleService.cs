using Inventory.Core.Dtos;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Services
{
    public class ModuleService : IModuleService
    {
        private readonly InventoryDbContext _context;

        public ModuleService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModuleDto>> GetModulesByUserIdAsync(int userId)
        {
            // 1. Obtener roles del usuario
            var userRoleIds = await _context.UserRoles
                .Where(ur => ur.IdUsers == userId)
                .Select(ur => ur.IdRoles)
                .ToListAsync();

            if (!userRoleIds.Any())
                return new List<ModuleDto>();

            // 2. Obtener permisos del usuario
            var userPermissionIds = await _context.RolePermissions
                .Where(rp => userRoleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .Distinct()
                .ToListAsync();

            // 3. Obtener módulos a los que tiene acceso
            var modules = await _context.Modules
                .Where(m => m.IsActive)
                .Where(m => m.Permissions.Any(p =>
                    p.IsActive && userPermissionIds.Contains(p.IdPermission)))
                .Include(m => m.ChildModules)
                .ThenInclude(cm => cm.Permissions)
                .Include(m => m.Permissions)
                .OrderBy(m => m.Order) // ✅ Usar Order en lugar de Order
                .ToListAsync();

            // 4. Convertir a DTO
            return modules.Select(m => new ModuleDto
            {
                Id = m.IdModule,
                Name = m.Name,
                Description = m.Description,
                Route = m.Route,
                Icon = m.Icon,
                Order = m.Order, // ✅ Usar Order en lugar de Order
                Children = m.ChildModules
                    .Where(cm => cm.IsActive)
                    .Where(cm => cm.Permissions.Any(p =>
                        p.IsActive && userPermissionIds.Contains(p.IdPermission)))
                    .Select(cm => new ModuleDto
                    {
                        Id = cm.IdModule,
                        Name = cm.Name,
                        Description = cm.Description,
                        Route = cm.Route,
                        Icon = cm.Icon,
                        Order = cm.Order // ✅ Usar Order en lugar de Order
                    })
                    .OrderBy(cm => cm.Order) // ✅ Usar Order en lugar de Order
                    .ToList()
            });
        }

        public async Task<bool> HasPermissionAsync(int userId, string permissionCode)
        {
            // 1. Obtener roles del usuario
            var userRoleIds = await _context.UserRoles
                .Where(ur => ur.IdUsers == userId)
                .Select(ur => ur.IdRoles)
                .ToListAsync();

            if (!userRoleIds.Any())
                return false;

            // 2. Verificar si tiene el permiso
            return await _context.RolePermissions
                .Include(rp => rp.Permission)
                .AnyAsync(rp => userRoleIds.Contains(rp.RoleId) &&
                              rp.Permission.Code == permissionCode &&
                              rp.Permission.IsActive);
        }
    }
}