using System.Security.Cryptography;

namespace AuthService.Application.Helpers
{
    /// <summary>
    /// Provides secure password hashing and verification using PBKDF2 with SHA256.
    /// </summary>
    public static class PasswordHelper
    {
        private const int SaltSize = 16;        // 128-bit salt
        private const int KeySize = 32;         // 256-bit key
        private const int Iterations = 100_000; // recommended minimum for PBKDF2

        /// <summary>
        /// Generates a cryptographically secure password hash using PBKDF2-SHA256.
        /// The output format is: {iterations}.{salt}.{hash}
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        /// <summary>
        /// Verifies whether a given password matches the stored hashed value.
        /// Returns true if the password is valid; otherwise false.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            try
            {
                var parts = storedHash.Split('.', 3);
                if (parts.Length != 3)
                    return false;

                var iterations = int.Parse(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var storedKey = Convert.FromBase64String(parts[2]);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                var computedKey = pbkdf2.GetBytes(KeySize);

                // Prevent timing attacks
                return CryptographicOperations.FixedTimeEquals(storedKey, computedKey);
            }
            catch
            {
                return false;
            }
        }
    }
}
