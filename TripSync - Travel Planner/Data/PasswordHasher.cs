using System.Security.Cryptography;

namespace TripSync___Travel_Planner.Data
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;
        private const string Algorithm = "pbkdf2_sha256";

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return string.Join(
                '$',
                Algorithm,
                Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(key));
        }

        public static bool Verify(string password, string storedPassword)
        {
            if (!storedPassword.StartsWith($"{Algorithm}$", StringComparison.Ordinal))
                return password == storedPassword;

            var parts = storedPassword.Split('$');
            if (parts.Length != 4 || !int.TryParse(parts[1], out var iterations))
                return false;

            byte[] salt;
            byte[] expectedKey;
            try
            {
                salt = Convert.FromBase64String(parts[2]);
                expectedKey = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }
            var actualKey = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedKey.Length);

            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }

        public static bool NeedsUpgrade(string storedPassword)
        {
            return !storedPassword.StartsWith($"{Algorithm}${Iterations}$", StringComparison.Ordinal);
        }
    }
}
