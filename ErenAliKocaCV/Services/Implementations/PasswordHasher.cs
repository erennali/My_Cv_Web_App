using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using ErenAliKocaCV.Services.Interfaces;

namespace ErenAliKocaCV.Services.Implementations
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8; // 16 bytes
        private const int KeySize = 256 / 8; // 32 bytes
        private const int Iterations = 10000;
        private const char Delimiter = ':';
        
        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            
            // Hash the password with PBKDF2
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Iterations,
                numBytesRequested: KeySize);
            
            // Format as "iterations:base64(salt):base64(hash)"
            return $"{Iterations}{Delimiter}{Convert.ToBase64String(salt)}{Delimiter}{Convert.ToBase64String(hash)}";
        }
        
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            // Split the hash into its components
            string[] parts = hashedPassword.Split(Delimiter);
            if (parts.Length != 3)
            {
                return false; // The hash is invalid
            }
            
            // Verify the format
            if (!int.TryParse(parts[0], out int iterations))
            {
                return false; // Invalid format
            }
            
            try
            {
                // Extract the salt and the hash
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] hash = Convert.FromBase64String(parts[2]);
                
                // Hash the provided password with the same parameters
                byte[] providedHash = KeyDerivation.Pbkdf2(
                    password: providedPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: iterations,
                    numBytesRequested: hash.Length);
                
                // Compare the hashes
                return CryptographicOperations.FixedTimeEquals(hash, providedHash);
            }
            catch
            {
                return false; // An error occurred (e.g., invalid base64 data)
            }
        }
    }
} 