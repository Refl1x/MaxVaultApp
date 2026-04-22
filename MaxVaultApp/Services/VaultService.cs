using MaxVaultApp.Models;

namespace MaxVaultApp.Services;

public sealed class VaultService
{
    private readonly string _appFolder;
    private readonly string _vaultPath;
    private readonly string _backupFolder;
    private string _masterPassword = string.Empty;

    public VaultData Vault { get; private set; } = new();

    public VaultService()
    {
        _appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MaxVaultApp");
        _vaultPath = Path.Combine(_appFolder, "vault.dat");
        _backupFolder = Path.Combine(_appFolder, "Backups");

        Directory.CreateDirectory(_appFolder);
        Directory.CreateDirectory(_backupFolder);
    }

    public string VaultPath => _vaultPath;
    public string BackupFolder => _backupFolder;
    public bool HasVault => File.Exists(_vaultPath);

    public void CreateNewVault(string masterPassword)
    {
        _masterPassword = masterPassword;
        Vault = new VaultData();
        Save();
    }

    public bool Unlock(string masterPassword)
    {
        if (!HasVault) return false;

        var package = CryptoService.LoadPackage(_vaultPath);
        Vault = CryptoService.DecryptVault(package, masterPassword);
        _masterPassword = masterPassword;
        return true;
    }

    public void Save()
    {
        EnsureUnlocked();
        Vault.UpdatedAt = DateTime.Now;
        var package = CryptoService.EncryptVault(Vault, _masterPassword);
        CryptoService.SavePackage(_vaultPath, package);
    }

    public string ExportEncryptedBackup(string? destinationFolder = null)
    {
        EnsureUnlocked();
        destinationFolder ??= _backupFolder;
        Directory.CreateDirectory(destinationFolder);

        var backupPath = Path.Combine(destinationFolder, $"vault_backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak");
        Save();
        File.Copy(_vaultPath, backupPath, overwrite: true);
        return backupPath;
    }

    public void ImportEncryptedBackup(string backupPath)
    {
        if (!File.Exists(backupPath))
            throw new FileNotFoundException("Файл резервной копии не найден.", backupPath);

        File.Copy(backupPath, _vaultPath, overwrite: true);
    }

    public List<PasswordEntry> Search(string? text, string? category)
    {
        IEnumerable<PasswordEntry> query = Vault.Entries;

        if (!string.IsNullOrWhiteSpace(text))
        {
            query = query.Where(x =>
                x.ServiceName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                x.Login.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                x.Notes.Contains(text, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(category) && category != "Все")
            query = query.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        return query.OrderBy(x => x.ServiceName).ToList();
    }

    public Dictionary<string, int> CategoryStats()
    {
        return Vault.Entries
            .GroupBy(x => x.Category)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public List<PasswordEntry> GetOldPasswords() => Vault.Entries.Where(x => x.IsOlderThanSixMonths).OrderBy(x => x.LastChangedAt).ToList();

    public List<List<PasswordEntry>> GetDuplicatePasswordGroups()
    {
        return Vault.Entries
            .Where(x => !string.IsNullOrWhiteSpace(x.Password))
            .GroupBy(x => x.Password)
            .Where(g => g.Count() > 1)
            .Select(g => g.ToList())
            .ToList();
    }

    private void EnsureUnlocked()
    {
        if (string.IsNullOrWhiteSpace(_masterPassword))
            throw new InvalidOperationException("Хранилище не разблокировано.");
    }
}
