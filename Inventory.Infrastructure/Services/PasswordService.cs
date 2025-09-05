using System.Security.Cryptography;
using System.Text;
using Inventory.Core.Interfaces;

namespace Inventory.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public void CreatePasswordHash(string password, out byte[] passwordHash)
    {
        // Usar SHA256 para coincidir con el tamaño varbinary(30)
        using var sha256 = SHA256.Create();
        passwordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

        // Asegurar que no exceda 30 bytes (tamaño de tu columna)
        if (passwordHash.Length > 30)
        {
            Array.Resize(ref passwordHash, 30);
        }
    }

    public bool VerifyPasswordHash(string password, byte[] storedHash)
    {
        using var sha256 = SHA256.Create();
        var computedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

        // Ajustar tamaño si es necesario
        if (computedHash.Length > 30)
        {
            Array.Resize(ref computedHash, 30);
        }

        return computedHash.SequenceEqual(storedHash);
    }
}