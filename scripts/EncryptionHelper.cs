using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Provides AES-256 symmetric encryption and decryption utilities to protect local save files from tampering.
/// </summary>
public static class EncryptionHelper
{
    // 32-byte (256-bit) secret key required for AES-256 encryption
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("A1b2C3d4E5f6G7h8I9j0K1l2M3n4O5p6");

    /// <summary>
    /// Encrypts a plaintext string using AES with a randomly generated Initialization Vector (IV).
    /// </summary>
    public static string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.GenerateIV(); // Generate a unique IV for every save operation
        byte[] iv = aes.IV;

        using MemoryStream ms = new MemoryStream();
        
        // Prepend the 16-byte IV to the output stream so it can be extracted during decryption
        ms.Write(iv, 0, iv.Length);

        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (StreamWriter sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        // Return combined IV + ciphertext encoded as Base64
        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    /// Decrypts a Base64-encoded AES ciphertext string back into plaintext.
    /// </summary>
    public static string Decrypt(string encryptedText)
    {
        byte[] fullCipher = Convert.FromBase64String(encryptedText);

        using Aes aes = Aes.Create();
        aes.Key = Key;

        // Extract the first 16 bytes as the Initialization Vector (IV)
        byte[] iv = new byte[16];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        // Decrypt the remaining bytes following the IV
        using MemoryStream ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
        using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}