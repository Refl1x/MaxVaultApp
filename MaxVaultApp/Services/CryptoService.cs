using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MaxVaultApp.Models;

namespace MaxVaultApp.Services;

public static class CryptoService
{
    private const int SaltSize = 16;
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 200_000;

    public sealed class EncryptedPackage
    {
        public string Version { get; set; } = "1.0";
        public byte[] Salt { get; set; } = Array.Empty<byte>();
        public byte[] Nonce { get; set; } = Array.Empty<byte>();
        public byte[] CipherText { get; set; } = Array.Empty<byte>();
        public byte[] Tag { get; set; } = Array.Empty<byte>();
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public static EncryptedPackage EncryptVault(VaultData vault, string masterPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var key = DeriveKey(masterPassword, salt);
        var plaintext = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(vault, JsonOptions()));
        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        return new EncryptedPackage
        {
            Salt = salt,
            Nonce = nonce,
            CipherText = ciphertext,
            Tag = tag,
            UpdatedAt = DateTime.Now
        };
    }

    public static VaultData DecryptVault(EncryptedPackage package, string masterPassword)
    {
        var key = DeriveKey(masterPassword, package.Salt);
        var plaintext = new byte[package.CipherText.Length];

        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(package.Nonce, package.CipherText, package.Tag, plaintext);

        return JsonSerializer.Deserialize<VaultData>(plaintext, JsonOptions()) ?? new VaultData();
    }

    public static void SavePackage(string path, EncryptedPackage package)
    {
        var json = JsonSerializer.Serialize(package, JsonOptions());
        File.WriteAllText(path, json, Encoding.UTF8);
    }

    public static EncryptedPackage LoadPackage(string path)
    {
        var json = File.ReadAllText(path, Encoding.UTF8);
        return JsonSerializer.Deserialize<EncryptedPackage>(json, JsonOptions())
               ?? throw new InvalidOperationException("Не удалось прочитать зашифрованное хранилище.");
    }

    public static bool TryValidatePassword(string path, string masterPassword)
    {
        try
        {
            var package = LoadPackage(path);
            _ = DecryptVault(package, masterPassword);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static byte[] DeriveKey(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
    }

    private static JsonSerializerOptions JsonOptions() => new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
}
