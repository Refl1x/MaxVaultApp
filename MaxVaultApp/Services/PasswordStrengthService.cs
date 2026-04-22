using System.Text.RegularExpressions;

namespace MaxVaultApp.Services;

public static class PasswordStrengthService
{
    private static readonly string[] WeakPatterns = ["1234", "password", "qwerty", "admin", "1111", "0000"];

    public static (int Score, string Label, string Message) Analyze(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (0, "Пустой", "Введите пароль.");

        var score = 0;
        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;
        if (Regex.IsMatch(password, "[a-z]")) score++;
        if (Regex.IsMatch(password, "[A-Z]")) score++;
        if (Regex.IsMatch(password, "\\d")) score++;
        if (Regex.IsMatch(password, "[^a-zA-Z0-9]")) score++;

        if (WeakPatterns.Any(p => password.Contains(p, StringComparison.OrdinalIgnoreCase)))
            score = Math.Min(score, 1);

        return score switch
        {
            <= 2 => (score, "Слабый", "Пароль слишком слабый. Лучше сгенерировать новый."),
            <= 4 => (score, "Средний", "Неплохо, но можно усилить символами и длиной."),
            _ => (score, "Сильный", "Хороший пароль для безопасного хранения.")
        };
    }

    public static bool IsWeak(string password) => Analyze(password).Label == "Слабый";
}
