using System.Security.Cryptography;
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace BrugerServiceApi.Services;

public class HashingService
{
    public byte[] CreateSalt ()   
    {
        byte[] salt = new byte[16];
        using (var randomGenerator = new RNGCryptoServiceProvider())
        {
            randomGenerator.GetBytes(salt);
        }
        return salt;
    }

    public byte[] HashPassword(byte[] password, byte[] salt)
    {
        int iterations = 1000;
        int hashByteSize = 32;

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
        {
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(hashByteSize);
        }
    }
}