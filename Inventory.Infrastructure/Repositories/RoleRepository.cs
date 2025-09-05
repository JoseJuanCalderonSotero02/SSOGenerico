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

public class RoleRepository : IRoleRepository
{
    private readonly InventoryDbContext _context;

    public RoleRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<Role> GetByIdAsync(int id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<Role> GetByCodeAsync(string code)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Code == code);
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }
}