using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
    Task<bool> ValidateTokenAsync(string token);
    Task<int?> GetUserIdFromTokenAsync(string token);
}