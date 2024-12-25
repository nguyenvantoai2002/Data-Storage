using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class DataEncryptor
{
    private static readonly byte[] DefaultKey = Encoding.UTF8.GetBytes("1234567890123456"); // Key 16 bytes
    private static readonly byte[] DefaultIV = Encoding.UTF8.GetBytes("6543210987654321"); // IV 16 bytes

    public static byte[] Encrypt(string plainText, byte[] key = null, byte[] iv = null)
    {
        key ??= DefaultKey;
        iv ??= DefaultIV;

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new StreamWriter(cs, Encoding.UTF8))
        {
            writer.Write(plainText);
        }
        return ms.ToArray();
    }

    public static string Decrypt(byte[] cipherText, byte[] key = null, byte[] iv = null)
    {
        key ??= DefaultKey;
        iv ??= DefaultIV;

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}