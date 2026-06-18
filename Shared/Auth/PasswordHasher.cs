using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Auth
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private static readonly PasswordHasher<object> Hasher = new();
        private static readonly object Dummy = new();

        public string Hash(string password) => Hasher.HashPassword(Dummy, password);

        public bool Verify(string passwordHash, string password)
        {
            if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(password))
                return false;

            var result = Hasher.VerifyHashedPassword(Dummy, passwordHash, password);
            return result is PasswordVerificationResult.Success
                or PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
