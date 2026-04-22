using MaxVaultApp.Services;
using MaxVaultApp.UI;

namespace MaxVaultApp;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        var vaultService = new VaultService();
        using var login = new LoginForm(vaultService);
        if (login.ShowDialog() == DialogResult.OK)
        {
            Application.Run(new MainForm(vaultService));
        }
    }
}
