using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces;

public interface IRoleRepository
{
    Task<Role> GetByIdAsync(int id);
    Task<Role> GetByCodeAsync(string code);
    Task<IEnumerable<Role>> GetAllAsync();
}