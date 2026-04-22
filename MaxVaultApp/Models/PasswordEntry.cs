namespace MaxVaultApp.Models;

public class PasswordEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ServiceName { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Category { get; set; } = "Личное";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastChangedAt { get; set; } = DateTime.Now;
    public string Notes { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }

    public bool IsOlderThanSixMonths => LastChangedAt <= DateTime.Now.AddMonths(-6);
}
