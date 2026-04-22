using MaxVaultApp.Models;
using MaxVaultApp.Services;

namespace MaxVaultApp.UI;

public sealed class EntryEditorForm : Form
{
    private readonly TextBox _txtService = new();
    private readonly TextBox _txtLogin = new();
    private readonly TextBox _txtPassword = new();
    private readonly ComboBox _cmbCategory = new();
    private readonly DateTimePicker _dtCreated = new();
    private readonly TextBox _txtNotes = new();
    private readonly CheckBox _chkFavorite = new();
    private readonly Label _lblStrength = new();
    private readonly NumericUpDown _numLength = new();
    private readonly CheckBox _chkLetters = new();
    private readonly CheckBox _chkDigits = new();
    private readonly CheckBox _chkSymbols = new();
    private readonly Button _btnGenerate = new();
    private readonly Button _btnSave = new();
    private readonly Panel _shell = new();

    public PasswordEntry Entry { get; private set; }

    public EntryEditorForm(PasswordEntry? entry = null)
    {
        Entry = entry is null
            ? new PasswordEntry { CreatedAt = DateTime.Now, LastChangedAt = DateTime.Now }
            : new PasswordEntry
            {
                Id = entry.Id,
                ServiceName = entry.ServiceName,
                Login = entry.Login,
                Password = entry.Password,
                Category = entry.Category,
                CreatedAt = entry.CreatedAt,
                LastChangedAt = entry.LastChangedAt,
                Notes = entry.Notes,
                IsFavorite = entry.IsFavorite
            };

        InitializeComponent();
        BindData();
    }

    private void InitializeComponent()
    {
        Text = "Карточка";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Size = new Size(700, 560);
        MaximizeBox = false;
        BackColor = ThemeManager.Background;
        Font = ThemeManager.CreateFont(10.2F);
        DoubleBuffered = true;

        _shell.SetBounds(18, 18, 648, 486);
        _shell.BackColor = Color.Transparent;
        _shell.Paint += (_, e) => ThemeManager.DrawPanel(e.Graphics, new Rectangle(0, 0, _shell.Width - 1, _shell.Height - 1), 24);

        var lblTitle = new Label { Text = "Карточка доступа", Left = 26, Top = 18, Width = 240, Font = ThemeManager.CreateFont(18F, FontStyle.Bold) };
        var lblHint = new Label { Text = "Сервис, логин, пароль и параметры безопасности", Left = 28, Top = 52, Width = 360, ForeColor = ThemeManager.TextMuted };

        var lblService = new Label { Text = "Сервис", Left = 28, Top = 94, Width = 120 };
        _txtService.SetBounds(28, 118, 270, 32);

        var lblLogin = new Label { Text = "Логин", Left = 320, Top = 94, Width = 120 };
        _txtLogin.SetBounds(320, 118, 296, 32);

        var lblPassword = new Label { Text = "Пароль", Left = 28, Top = 160, Width = 120 };
        _txtPassword.SetBounds(28, 184, 270, 32);
        _txtPassword.TextChanged += (_, _) => UpdateStrength();

        _lblStrength.SetBounds(320, 186, 296, 24);
        _lblStrength.ForeColor = ThemeManager.Warning;

        var lblCategory = new Label { Text = "Категория", Left = 28, Top = 226, Width = 120 };
        _cmbCategory.SetBounds(28, 250, 184, 34);
        _cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbCategory.Items.AddRange(["Личное", "Работа", "Финансы"]);

        var lblCreated = new Label { Text = "Создан", Left = 220, Top = 226, Width = 120 };
        _dtCreated.SetBounds(220, 250, 170, 32);
        _dtCreated.Format = DateTimePickerFormat.Short;

        _chkFavorite.Text = "Важный пароль ★";
        _chkFavorite.SetBounds(418, 254, 170, 24);

        var grpGenerator = new GroupBox { Text = "Генератор", Left = 28, Top = 300, Width = 588, Height = 92 };
        _numLength.SetBounds(18, 36, 58, 28);
        _numLength.Minimum = 8;
        _numLength.Maximum = 20;
        _numLength.Value = 14;

        _chkLetters.Text = "Буквы";
        _chkLetters.SetBounds(96, 38, 72, 22);
        _chkLetters.Checked = true;
        _chkDigits.Text = "Цифры";
        _chkDigits.SetBounds(176, 38, 72, 22);
        _chkDigits.Checked = true;
        _chkSymbols.Text = "Символы";
        _chkSymbols.SetBounds(256, 38, 88, 22);
        _chkSymbols.Checked = true;

        _btnGenerate.Text = "Сгенерировать";
        _btnGenerate.SetBounds(428, 33, 136, 30);
        _btnGenerate.Click += (_, _) => GeneratePassword();
        grpGenerator.Controls.AddRange([_numLength, _chkLetters, _chkDigits, _chkSymbols, _btnGenerate]);

        var lblNotes = new Label { Text = "Заметки", Left = 28, Top = 404, Width = 120 };
        _txtNotes.SetBounds(28, 428, 430, 32);

        _btnSave.Text = "Сохранить";
        _btnSave.SetBounds(480, 426, 136, 34);
        _btnSave.Click += (_, _) => SaveEntry();

        _shell.Controls.AddRange([
            lblTitle, lblHint, lblService, _txtService, lblLogin, _txtLogin, lblPassword, _txtPassword, _lblStrength,
            lblCategory, _cmbCategory, lblCreated, _dtCreated, _chkFavorite, grpGenerator, lblNotes, _txtNotes, _btnSave
        ]);

        Controls.Add(_shell);
        ThemeManager.ApplyTheme(this);
        ThemeManager.EnableFadeIn(this);
        Shown += (_, _) => ApplyRounding();
        Resize += (_, _) => ApplyRounding();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        ThemeManager.DrawGlow(e.Graphics, new Rectangle(420, -10, 180, 110));
    }

    private void ApplyRounding()
    {
        ThemeManager.RoundControl(_btnSave, 12);
        ThemeManager.RoundControl(_btnGenerate, 12);
    }

    private void BindData()
    {
        _txtService.Text = Entry.ServiceName;
        _txtLogin.Text = Entry.Login;
        _txtPassword.Text = Entry.Password;
        _cmbCategory.SelectedItem = Entry.Category;
        if (_cmbCategory.SelectedIndex < 0) _cmbCategory.SelectedIndex = 0;
        _dtCreated.Value = Entry.CreatedAt == default ? DateTime.Now : Entry.CreatedAt;
        _txtNotes.Text = Entry.Notes;
        _chkFavorite.Checked = Entry.IsFavorite;
        UpdateStrength();
    }

    private void GeneratePassword()
    {
        try
        {
            _txtPassword.Text = PasswordGeneratorService.Generate((int)_numLength.Value, _chkLetters.Checked, _chkDigits.Checked, _chkSymbols.Checked);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Генератор", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void UpdateStrength()
    {
        var result = PasswordStrengthService.Analyze(_txtPassword.Text);
        _lblStrength.Text = $"Надёжность: {result.Label}";
        _lblStrength.ForeColor = result.Label switch
        {
            "Слабый" => ThemeManager.Danger,
            "Средний" => ThemeManager.Warning,
            _ => ThemeManager.Success
        };
    }

    private void SaveEntry()
    {
        if (string.IsNullOrWhiteSpace(_txtService.Text) || string.IsNullOrWhiteSpace(_txtLogin.Text) || string.IsNullOrWhiteSpace(_txtPassword.Text))
        {
            MessageBox.Show("Заполните сервис, логин и пароль.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (PasswordStrengthService.IsWeak(_txtPassword.Text))
        {
            var result = MessageBox.Show(
                "Пароль слабый. Сгенерировать новый?",
                "Слабый пароль",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                GeneratePassword();
                return;
            }
            if (result == DialogResult.Cancel)
                return;
        }

        var previousPassword = Entry.Password;
        Entry.ServiceName = _txtService.Text.Trim();
        Entry.Login = _txtLogin.Text.Trim();
        Entry.Password = _txtPassword.Text;
        Entry.Category = _cmbCategory.SelectedItem?.ToString() ?? "Личное";
        Entry.CreatedAt = _dtCreated.Value.Date;
        Entry.Notes = _txtNotes.Text.Trim();
        Entry.IsFavorite = _chkFavorite.Checked;
        if (!string.Equals(previousPassword, Entry.Password, StringComparison.Ordinal))
            Entry.LastChangedAt = DateTime.Now;

        DialogResult = DialogResult.OK;
        Close();
    }
}
