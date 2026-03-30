
using BusinessCentral.Application.Ports.Outbound;

namespace BusinessCentral.Infrastructure.Security
{
    public class HashPasswordService : IHashPasswordService
    {
        // Método para encriptar la contraseña
        public string Hash(string password)
        {
            // El método HashPassword genera un hash a partir de la contraseña
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Método para verificar si una contraseña coincide con su hash
        public bool Verify(string password, string hashedPassword)
        {
            // Verifica si la contraseña en texto plano coincide con el hash almacenado
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
