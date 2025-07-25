using System.Security.Cryptography;

namespace ImageEntropy;

/// <summary>
/// Provides AES encryption and decryption for byte arrays using password-based key derivation.
/// </summary>
public static class Helper
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int IvSize = 16;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA256;

    /// <summary>
    /// Encrypts a byte array with AES using the given password.
    /// </summary>
    public static byte[] Encrypt(byte[] plainBytes, string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;

        using var keyDerivation = new Rfc2898DeriveBytes(password, salt, Iterations, _hashAlgorithm);
        aes.Key = keyDerivation.GetBytes(KeySize);
        aes.IV = keyDerivation.GetBytes(IvSize);

        using var memoryStream = new MemoryStream();
        memoryStream.Write(salt, 0, salt.Length);

        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
        }
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Decrypts an encrypted byte array (prefixed with a salt).
    /// </summary>
    public static byte[] Decrypt(byte[] encryptedBytesWithSalt, string password)
    {
        byte[] salt = new byte[SaltSize];
        Array.Copy(encryptedBytesWithSalt, 0, salt, 0, SaltSize);

        byte[] encryptedData = new byte[encryptedBytesWithSalt.Length - SaltSize];
        Array.Copy(encryptedBytesWithSalt, SaltSize, encryptedData, 0, encryptedData.Length);

        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;

        using var keyDerivation = new Rfc2898DeriveBytes(password, salt, Iterations, _hashAlgorithm);
        aes.Key = keyDerivation.GetBytes(KeySize);
        aes.IV = keyDerivation.GetBytes(IvSize);

        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            cryptoStream.Write(encryptedData, 0, encryptedData.Length);
        }
        return memoryStream.ToArray();
    }
}