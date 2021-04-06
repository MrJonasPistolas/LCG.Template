using LCG.Template.ServiceContracts;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LCG.Template.Services
{
    public class CypherService : ICypherService
    {
        private readonly IConfiguration _configuration;

        public CypherService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Decrypt(string text)
        {
            var key = _configuration["CypherKey"];

            var buffer = Encoding.UTF8.GetBytes(text);
            var hash = new SHA512CryptoServiceProvider();
            var aesKey = new byte[24];
            Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new ArgumentException("Parameter must not be null.", nameof(aes));

                aes.Key = aesKey;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var resultStream = new MemoryStream())
                {
                    using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(buffer))
                    {
                        plainStream.CopyTo(aesStream);
                    }

                    var result = resultStream.ToArray();
                    var combined = new byte[aes.IV.Length + result.Length];
                    Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
                    Array.ConstrainedCopy(result, 0, combined, aes.IV.Length, result.Length);

                    return Convert.ToBase64String(combined);
                }
            }
        }

        public string Encrypt(string encryptedText)
        {
            var key = _configuration["CypherKey"];

            var combined = Convert.FromBase64String(encryptedText);
            var buffer = new byte[combined.Length];
            var hash = new SHA512CryptoServiceProvider();
            var aesKey = new byte[24];
            Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new ArgumentException("Parameter must not be null.", nameof(aes));

                aes.Key = aesKey;

                var iv = new byte[aes.IV.Length];
                var ciphertext = new byte[buffer.Length - iv.Length];

                Array.ConstrainedCopy(combined, 0, iv, 0, iv.Length);
                Array.ConstrainedCopy(combined, iv.Length, ciphertext, 0, ciphertext.Length);

                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var resultStream = new MemoryStream())
                {
                    using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(ciphertext))
                    {
                        plainStream.CopyTo(aesStream);
                    }

                    return Encoding.UTF8.GetString(resultStream.ToArray());
                }
            }
        }
    }
}
