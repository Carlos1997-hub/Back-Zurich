using System.Security.Cryptography;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        // PBKDF2 settings (buen balance para prueba)
        private const int SaltSize = 32;
        private const int HashSize = 64;
        private const int Iterations = 100_000;

        public void CreateHash(string password, out byte[] hash, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(SaltSize);
            hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize
            );
        }

        public bool Verify(string password, byte[] hash, byte[] salt)
        {
            var computed = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                hash.Length
            );

            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
    }
}
