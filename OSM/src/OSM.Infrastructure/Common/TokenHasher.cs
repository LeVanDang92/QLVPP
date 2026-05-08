using System.Security.Cryptography;
using System.Text;

namespace OSM.Infrastructure.Common
{
    public static class TokenHasher
    {
        public static string HashToken(string token, string pepper)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            if (string.IsNullOrWhiteSpace(pepper))
                throw new ArgumentException("Pepper cannot be null or empty.", nameof(pepper));

            var keyBytes = Encoding.UTF8.GetBytes(pepper);
            var tokenBytes = Encoding.UTF8.GetBytes(token);

            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(tokenBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
