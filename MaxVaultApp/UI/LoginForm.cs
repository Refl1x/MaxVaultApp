using MaxVaultApp.Services;

namespace MaxVaultApp.UI;

public sealed class LoginForm : Form
{
    private readonly VaultService _vaultService;
    private readonly Panel _leftPanel = new();
    private readonly Panel _card = new();
    private readonly Label _brandChip = new();
    private readonly Label _brandTitle = new();
    private readonly Label _brandText = new();
    private readonly Label _feature1 = new();
    private readonly Label _feature2 = new();
    private readonly Label _feature3 = new();
    private readonly Label _title = new();
    private readonly Label _subtitle = new();
    private readonly Label _passwordLabel = new();
    private readonly Label _confirmLabel = new();
    private readonly Label _statusChip = new();
    private readonly TextBox _txtPassword = new();
    private readonly TextBox _txtConfirm = new();
    private readonly Button _btnOpen = new();

    public LoginForm(VaultService vaultService)
    {
        _vaultService = vaultService;
        InitializeComponent();
        UpdateMode();
    }

    private void InitializeComponent()
    {
        Text = "MaxVault";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        Width = 820;
        Height = 520;
        BackColor = ThemeManager.Background;
        Font = ThemeManager.CreateFont(10.2F);
        DoubleBuffered = true;

        _leftPanel.SetBounds(28, 28, 292, 424);
        _leftPanel.BackColor = Color.Transparent;
        _leftPanel.Paint += (_, e) => ThemeManager.DrawGlassPanel(e.Graphics, new Rectangle(0, 0, _leftPanel.Width - 1, _leftPanel.Height - 1), 30);

        _brandChip.Text = "PREMIUM • LOCAL";
        _brandChip.ForeColor = ThemeManager.Accent2;
        _brandChip.Font = ThemeManager.CreateFont(9F, FontStyle.Bold);
        _brandChip.SetBounds(24, 28, 180, 20);

        _brandTitle.Text = "MaxVault";
        _brandTitle.Font = ThemeManager.CreateFont(28F, FontStyle.Bold);
        _brandTitle.SetBounds(22, 58, 220, 48);

        _brandText.Text = "Ваш офлайн-сейф для логинов, банковских сервисов и рабочих аккаунтов.";
        _brandText.ForeColor = ThemeManager.TextMuted;
        _brandText.SetBounds(24, 112, 236, 52);

        _feature1.Text = "◈  Локальное шифрование без облаков";
        _feature2.Text = "✦  Контроль старых и повторяющихся паролей";
        _feature3.Text = "★  Избранные записи и зашифрованный backup";

        foreach (var feature in new[] { _feature1, _feature2, _feature3 })
        {
            feature.ForeColor = ThemeManager.TextPrimary;
            feature.Font = ThemeManager.CreateFont(9.2F, FontStyle.Bold);
            feature.Width = 240;
            feature.Height = 40;
        }

        _feature1.SetBounds(24, 210, 240, 36);
        _feature2.SetBounds(24, 262, 240, 36);
        _feature3.SetBounds(24, 314, 240, 36);

        _leftPanel.Controls.AddRange([_brandChip, _brandTitle, _brandText, _feature1, _feature2, _feature3]);

        _card.SetBounds(350, 46, 428, 388);
        _card.BackColor = Color.Transparent;
        _card.Paint += (_, e) => ThemeManager.DrawPanel(e.Graphics, new Rectangle(0, 0, _card.Width - 1, _card.Height - 1), 30);

        _statusChip.Text = "SAFE ACCESS";
        _statusChip.ForeColor = ThemeManager.Accent2;
        _statusChip.Font = ThemeManager.CreateFont(8.8F, FontStyle.Bold);
        _statusChip.SetBounds(34, 24, 140, 20);

        _title.Text = "Вход в сейф";
        _title.Font = ThemeManager.CreateFont(23F, FontStyle.Bold);
        _title.SetBounds(32, 52, 240, 40);

        _subtitle.Font = ThemeManager.CreateFont(10.2F);
        _subtitle.ForeColor = ThemeManager.TextMuted;
        _subtitle.SetBounds(34, 92, 330, 40);

        _passwordLabel.Text = "Мастер-пароль";
        _passwordLabel.SetBounds(34, 148, 160, 20);

        _txtPassword.UseSystemPasswordChar = true;
        _txtPassword.SetBounds(34, 172, 360, 36);

        _confirmLabel.Text = "Повтор пароля";
        _confirmLabel.SetBounds(34, 220, 160, 20);

        _txtConfirm.UseSystemPasswordChar = true;
        _txtConfirm.SetBounds(34, 244, 360, 36);

        _btnOpen.Text = "✦  Открыть сейф";
        _btnOpen.SetBounds(34, 300, 172, 38);
        _btnOpen.Click += (_, _) => OpenVault();

        _card.Controls.AddRange([
            _statusChip, _title, _subtitle, _passwordLabel, _txtPassword,
            _confirmLabel, _txtConfirm, _btnOpen
        ]);

        Controls.AddRange([_leftPanel, _card]);
        ThemeManager.ApplyTheme(this);
        ThemeManager.EnableFadeIn(this);
        Shown += (_, _) => ApplyRounding();
        Resize += (_, _) => ApplyRounding();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        ThemeManager.DrawGlow(e.Graphics, new Rectangle(-60, -30, 300, 180));
        ThemeManager.DrawGlow(e.Graphics, new Rectangle(520, -20, 220, 140));
        ThemeManager.DrawGlow(e.Graphics, new Rectangle(620, 360, 160, 100));
    }

    private void ApplyRounding()
    {
        ThemeManager.RoundControl(_btnOpen, 14);
    }

    private void UpdateMode()
    {
        var firstRun = !_vaultService.HasVault;
        _subtitle.Text = firstRun
            ? "Создайте мастер-пароль, чтобы зашифровать локальный сейф и открыть приложение."
            : "Введите мастер-пароль для расшифровки и безопасного входа в хранилище.";

        _confirmLabel.Visible = firstRun;
        _txtConfirm.Visible = firstRun;
        _btnOpen.Text = firstRun ? "✦  Создать сейф" : "✦  Открыть сейф";
    }

    private void OpenVault()
    {
        var password = _txtPassword.Text.Trim();
        if (password.Length < 8)
        {
            MessageBox.Show("Мастер-пароль должен быть не короче 8 символов.", "MaxVault", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!_vaultService.HasVault)
        {
            if (password != _txtConfirm.Text)
            {
                MessageBox.Show("Пароли не совпадают.", "MaxVault", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _vaultService.CreateNewVault(password);
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        try
        {
            if (_vaultService.Unlock(password))
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
        }
        catch
        {
        }

        MessageBox.Show("Неверный мастер-пароль или повреждено хранилище.", "MaxVault", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
