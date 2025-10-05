using domain.DTOs;
using Microsoft.AspNetCore.Identity;

namespace domain.Wrapper
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<object> _hasher = new();

        public static string pwdHashPassword(string password) => _hasher.HashPassword(null, password);

        public static bool pwdVerifyPassword(string password, string hashed) =>
            _hasher.VerifyHashedPassword(null, hashed, password) is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
