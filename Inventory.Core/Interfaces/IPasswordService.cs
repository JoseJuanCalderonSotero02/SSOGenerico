using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Interfaces;
public interface IPasswordService
{
    void CreatePasswordHash(string password, out byte[] passwordHash);
    bool VerifyPasswordHash(string password, byte[] storedHash);
}