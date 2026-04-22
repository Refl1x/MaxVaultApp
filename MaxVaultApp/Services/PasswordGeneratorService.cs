using System.Security.Cryptography;

namespace MaxVaultApp.Services;

public static class PasswordGeneratorService
{
    private const string LettersLower = "abcdefghijklmnopqrstuvwxyz";
    private const string LettersUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string Symbols = "!@#$%^&*()-_=+[]{};:,.?/";

    public static string Generate(int length, bool useLetters = true, bool useDigits = true, bool useSymbols = true)
    {
        if (length < 8 || length > 20)
            throw new ArgumentOutOfRangeException(nameof(length), "Длина должна быть от 8 до 20 символов.");

        var pool = string.Empty;
        if (useLetters) pool += LettersLower + LettersUpper;
        if (useDigits) pool += Digits;
        if (useSymbols) pool += Symbols;

        if (string.IsNullOrWhiteSpace(pool))
            throw new InvalidOperationException("Нужно выбрать хотя бы один тип символов.");

        var chars = new char[length];
        for (var i = 0; i < length; i++)
        {
            chars[i] = pool[RandomNumberGenerator.GetInt32(pool.Length)];
        }

        return new string(chars);
    }
}
