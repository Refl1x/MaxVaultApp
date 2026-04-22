namespace MaxVaultApp.Models;

public class VaultData
{
    public List<PasswordEntry> Entries { get; set; } = new();
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
