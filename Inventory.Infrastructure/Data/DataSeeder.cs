using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Data;

public class DataSeeder
{
    private readonly InventoryDbContext _context;
    private readonly IPasswordService _passwordService;

    public DataSeeder(InventoryDbContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task SeedAsync()
    {
        await SeedRoles();
        await SeedAdminUser();
        await SeedStatuses();
    }

    private async Task SeedRoles()
    {
        if (!await _context.Roles.AnyAsync())
        {
            var roles = new List<Role>
        {
            new Role { Code = "ADMIN", Name = "Administrador", IsSystem = true },
            new Role { Code = "EDITOR", Name = "Editor", IsSystem = true },
            new Role { Code = "VIEWER", Name = "Visor", IsSystem = true },
            new Role { Code = "BRANCH_MANAGER", Name = "Gerente de Sucursal", IsSystem = true },
            new Role { Code = "SUPPLIER", Name = "Proveedor", IsSystem = true },
            new Role { Code = "AUDITOR", Name = "Auditor", IsSystem = true },
            new Role { Code = "USER", Name = "Usuario General", IsSystem = true }
        };

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedAdminUser()
    {
        if (!await _context.Users.AnyAsync(u => u.Username == "admin"))
        {
            _passwordService.CreatePasswordHash("Admin123!", out byte[] passwordHash);

            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@inventory.com",
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(adminUser);
            await _context.SaveChangesAsync();

            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Code == "ADMIN");
            if (adminRole != null)
            {
                var userRole = new UserRole
                {
                    IdUsers = adminUser.IdUsers,
                    IdRoles = adminRole.IdRoles,
                    AssignedDate = DateTime.UtcNow
                };

                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();
            }
        }
    }

    private async Task SeedStatuses()
    {
        if (!await _context.Statuses.AnyAsync())
        {
            var statuses = new List<Status>
            {
                // Estados para Requisiciones
                new Status { Entity = "Requisition", Name = "Borrador" },
                new Status { Entity = "Requisition", Name = "Pendiente Aprobación" },
                new Status { Entity = "Requisition", Name = "Aprobada" },
                new Status { Entity = "Requisition", Name = "Rechazada" },
                new Status { Entity = "Requisition", Name = "En Proceso" },
                
                // Estados para Órdenes de Compra
                new Status { Entity = "PurchaseOrder", Name = "Pendiente" },
                new Status { Entity = "PurchaseOrder", Name = "Confirmada" },
                new Status { Entity = "PurchaseOrder", Name = "En Preparación" },
                new Status { Entity = "PurchaseOrder", Name = "Enviada" },
                new Status { Entity = "PurchaseOrder", Name = "Recibida Parcialmente" },
                new Status { Entity = "PurchaseOrder", Name = "Recibida Completamente" },
                new Status { Entity = "PurchaseOrder", Name = "Facturada" },
                new Status { Entity = "PurchaseOrder", Name = "Pagada" },
                new Status { Entity = "PurchaseOrder", Name = "Cerrada" },
                new Status { Entity = "PurchaseOrder", Name = "Cancelada" }
            };

            await _context.Statuses.AddRangeAsync(statuses);
            await _context.SaveChangesAsync();
        }
    }
}