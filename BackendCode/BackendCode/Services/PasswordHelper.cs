using System;
using System.Security.Cryptography;

namespace BackendCode.Services
{
    public class PasswordHelper
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            var salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt); // 使用 RandomNumberGenerator.Fill 方法生成随机盐值

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                var key = pbkdf2.GetBytes(KeySize);
                var hash = new byte[SaltSize + KeySize];
                Array.Copy(salt, 0, hash, 0, SaltSize);
                Array.Copy(key, 0, hash, SaltSize, KeySize);

                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                var key = pbkdf2.GetBytes(KeySize);
                for (int i = 0; i < KeySize; i++)
                {
                    if (hashBytes[i + SaltSize] != key[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }





    }
}
