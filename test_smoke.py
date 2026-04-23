from pathlib import Path
ROOT = Path(__file__).resolve().parents[1]
def read(rel: str) -> str:
    return (ROOT / rel).read_text(encoding='utf-8')
def test_crypto_uses_aesgcm_and_pbkdf2():
    text = read('MaxVaultApp/Services/CryptoService.cs')
    assert 'AesGcm' in text
    assert 'Rfc2898DeriveBytes.Pbkdf2' in text
def test_password_generator_has_length_boundaries_and_required_sets():
    text = read('MaxVaultApp/Services/PasswordGeneratorService.cs')
    assert 'length < 8 || length > 20' in text
    assert 'selectedSets' in text
    assert 'Shuffle(chars)' in text
def test_password_entry_exposes_masked_password_property():
    text = read('MaxVaultApp/Models/PasswordEntry.cs')
    assert 'MaskedPassword' in text
    assert "new string('•'" in text
def test_main_form_binds_grid_to_masked_password():
    text = read('MaxVaultApp/UI/MainForm.cs')
    assert 'nameof(PasswordEntry.MaskedPassword)' in text
def test_backup_import_validates_package_before_replacing_vault():
    text = read('MaxVaultApp/Services/VaultService.cs')
    assert 'CryptoService.LoadPackage(backupPath)' in text
    assert 'CryptoService.DecryptVault(package, _masterPassword)' in text
    # validation should happen before file overwrite
    assert text.index('CryptoService.DecryptVault(package, _masterPassword)') < text.index('File.Copy(backupPath, _vaultPath, overwrite: true)')
