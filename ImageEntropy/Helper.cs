using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace ImageEntropy
{
    /// <summary>
    /// The <c>Helper</c> class.
    /// Performs encryption and decryption of a byte array.
    /// </summary>
    public class Helper
    {
        //16 bytes salt
        private static readonly byte[] salt = Encoding.Unicode.GetBytes("MySalt");
        //iterations for key derivation 
        private static readonly int iterations = 100_000;
        ///Encrypt a byte array using Aes block encryption
        public static string Encrypt(byte[] plainBytes, string password)
        {
            byte[] encryptedBytes;
            using (Aes aes = Aes.Create())
            {
                using (Rfc2898DeriveBytes pbkdf2 = new(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    aes.Padding = PaddingMode.Zeros;
                    aes.Key = pbkdf2.GetBytes(32); // 256-bit key                   
                    aes.IV = pbkdf2.GetBytes(16); // 128-bit IV
                }
                using (MemoryStream ms = new())
                {
                    using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(plainBytes, 0, plainBytes.Length);
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return Convert.ToBase64String(encryptedBytes);
        }
        ///Decrypt a byte array using Aes block encryption
        public static string Decrypt(byte[] cryptoBytes, string password)
        {
            byte[] plainBytes;
            using (Aes aes = Aes.Create())
            {
                using (Rfc2898DeriveBytes pbkdf2 = new(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    aes.Padding = PaddingMode.Zeros;
                    aes.Key = pbkdf2.GetBytes(32);
                    aes.IV = pbkdf2.GetBytes(16);
                }
                using (MemoryStream ms = new())
                {
                    using (CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cryptoBytes, 0, cryptoBytes.Length);
                    }
                    plainBytes = ms.ToArray();
                }
            }
            return Convert.ToBase64String(plainBytes);
        }
    }
}