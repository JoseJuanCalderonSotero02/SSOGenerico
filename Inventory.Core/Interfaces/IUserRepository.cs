using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<bool> UserExists(string username, string email);
}