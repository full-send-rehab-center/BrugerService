using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace BrugerServiceApi.Services;

public class HashingService
{
    public string CreateSalt ()   
    {
        byte[] saltBytes = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(saltBytes);
        }
        string salt = Convert.ToBase64String(saltBytes);
        return salt;
    }

    public string HashPassword(string password, string salt)
    {
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Convert.FromBase64String(salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return hashed;
    }
}