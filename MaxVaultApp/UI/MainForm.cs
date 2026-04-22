using MaxVaultApp.Models;
using MaxVaultApp.Services;
using System.Drawing.Drawing2D;
using System.Text;

namespace MaxVaultApp.UI;

public sealed class MainForm : Form
{
    private readonly VaultService _vaultService;

    private readonly Panel _sidebar = new();
    private readonly Panel _headerPanel = new();
    private readonly Panel _contentPanel = new();
    private readonly Panel _allView = new();
    private readonly Panel _favView = new();
    private readonly Panel _statsView = new();
    private readonly Panel _navIndicator = new();

    private readonly Label _lblLogo = new();
    private readonly Label _lblTag = new();
    private readonly Label _lblHero = new();
    private readonly Label _lblSummary = new();

    private readonly Button _btnNavAll = new();
    private readonly Button _btnNavFav = new();
    private readonly Button _btnNavStats = new();

    private readonly TextBox _txtSearch = new();
    private readonly ComboBox _cmbCategory = new();
    private readonly Button _btnAdd = new();
    private readonly Button _btnEdit = new();
    private readonly Button _btnDelete = new();
    private readonly Button _btnBackup = new();
    private readonly Button _btnRefresh = new();
    private readonly Button _btnImport = new();

    private readonly DataGridView _gridAll = new();
    private readonly DataGridView _gridFav = new();

    private readonly Panel _statsCardsHost = new();
    private readonly Panel _cardTotal = new();
    private readonly Panel _cardFav = new();
    private readonly Panel _cardOld = new();
    private readonly Panel _cardDuplicate = new();
    private readonly Label _cardTotalValue = new();
    private readonly Label _cardFavValue = new();
    private readonly Label _cardOldValue = new();
    private readonly Label _cardDuplicateValue = new();
    private readonly Panel _categoryChartPanel = new();
    private readonly Panel _ageChartPanel = new();
    private readonly Panel _alertsPanel = new();
    private readonly ListBox _lstAlerts = new();

    private Dictionary<string, int> _categoryStats = new();
    private List<PasswordEntry> _oldPasswords = new();
    private List<List<PasswordEntry>> _duplicateGroups = new();

    public MainForm(VaultService vaultService)
    {
        _vaultService = vaultService;
        InitializeComponent();
        LoadAll();
        ShowSection("all");
    }

    private void InitializeComponent()
    {
        Text = "MaxVault";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1360, 860);
        MinimumSize = new Size(1180, 760);
        BackColor = ThemeManager.Background;
        Font = ThemeManager.CreateFont(10.5F);
        DoubleBuffered = true;

        _sidebar.SetBounds(18, 18, 228, 786);
        _sidebar.BackColor = Color.Transparent;
        _sidebar.Paint += (_, e) => ThemeManager.DrawGlassPanel(e.Graphics, new Rectangle(0, 0, _sidebar.Width - 1, _sidebar.Height - 1), 28);

        _lblTag.Text = "OFFLINE • ENCRYPTED";
        _lblTag.ForeColor = ThemeManager.Accent2;
        _lblTag.Font = ThemeManager.CreateFont(8.8F, FontStyle.Bold);
        _lblTag.SetBounds(22, 28, 160, 18);

        _lblLogo.Text = "MaxVault";
        _lblLogo.Font = ThemeManager.CreateFont(24F, FontStyle.Bold);
        _lblLogo.SetBounds(20, 52, 180, 38);

        _navIndicator.SetBounds(14, 198, 4, 52);
        _navIndicator.BackColor = ThemeManager.Accent2;

        PrepareNavButton(_btnNavAll, "⌂  Все пароли", 190);
        PrepareNavButton(_btnNavFav, "★  Избранное", 248);
        PrepareNavButton(_btnNavStats, "◔  Статистика", 306);
        _btnNavAll.Click += (_, _) => ShowSection("all");
        _btnNavFav.Click += (_, _) => ShowSection("fav");
        _btnNavStats.Click += (_, _) => ShowSection("stats");

        _sidebar.Controls.AddRange([_lblTag, _lblLogo, _navIndicator, _btnNavAll, _btnNavFav, _btnNavStats]);

        _headerPanel.SetBounds(264, 18, 1078, 120);
        _headerPanel.BackColor = Color.Transparent;
        _headerPanel.Paint += (_, e) => ThemeManager.DrawPanel(e.Graphics, new Rectangle(0, 0, _headerPanel.Width - 1, _headerPanel.Height - 1), 26);

        _lblHero.Text = "Password bunker";
        _lblHero.Font = ThemeManager.CreateFont(21F, FontStyle.Bold);
        _lblHero.SetBounds(28, 14, 240, 34);

        _lblSummary.ForeColor = ThemeManager.TextMuted;
        _lblSummary.Font = ThemeManager.CreateFont(10.6F);
        _lblSummary.SetBounds(30, 58, 460, 22);

        _txtSearch.PlaceholderText = "Поиск по сервису";
        _txtSearch.SetBounds(474, 22, 192, 36);
        _txtSearch.TextChanged += (_, _) => RefreshGrid();

        _cmbCategory.SetBounds(676, 22, 126, 36);
        _cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbCategory.Items.AddRange(["Все", "Личное", "Работа", "Финансы"]);
        _cmbCategory.SelectedIndex = 0;
        _cmbCategory.SelectedIndexChanged += (_, _) => RefreshGrid();

        PrepareActionButton(_btnRefresh, "↻ Обновить", 392, 78, 112);
        PrepareActionButton(_btnAdd, "＋ Добавить", 512, 78, 110);
        PrepareActionButton(_btnEdit, "✎ Изменить", 630, 78, 112);
        PrepareActionButton(_btnDelete, "🗑 Удалить", 750, 78, 100);
        PrepareActionButton(_btnBackup, "⤓ Экспорт", 858, 78, 96);
        PrepareActionButton(_btnImport, "⤒ Импорт", 958, 78, 94);

        _btnRefresh.Click += (_, _) => LoadAll();
        _btnAdd.Click += (_, _) => AddEntry();
        _btnEdit.Click += (_, _) => EditSelected();
        _btnDelete.Click += (_, _) => DeleteSelected();
        _btnBackup.Click += (_, _) => ExportBackup();
        _btnImport.Click += (_, _) => ImportBackup();

        _headerPanel.Controls.AddRange([
            _lblHero, _lblSummary, _txtSearch, _cmbCategory,
            _btnRefresh, _btnAdd, _btnEdit, _btnDelete, _btnBackup, _btnImport
        ]);

        _contentPanel.SetBounds(264, 154, 1078, 650);
        _contentPanel.BackColor = Color.Transparent;
        _contentPanel.Paint += (_, e) => ThemeManager.DrawPanel(e.Graphics, new Rectangle(0, 0, _contentPanel.Width - 1, _contentPanel.Height - 1), 26);

        PrepareGridView(_gridAll);
        _gridAll.DoubleClick += (_, _) => EditSelected();
        _gridAll.SetBounds(18, 18, 1042, 612);
        _allView.SetBounds(0, 0, 1078, 650);
        _allView.BackColor = Color.Transparent;
        _allView.Controls.Add(_gridAll);

        PrepareGridView(_gridFav);
        _gridFav.SetBounds(18, 18, 1042, 612);
        _favView.SetBounds(0, 0, 1078, 650);
        _favView.BackColor = Color.Transparent;
        _favView.Controls.Add(_gridFav);

        BuildStatsView();

        _contentPanel.Controls.AddRange([_allView, _favView, _statsView]);
        Controls.AddRange([_sidebar, _headerPanel, _contentPanel]);

        ThemeManager.ApplyTheme(this);
        ThemeManager.StylePrimaryButton(_btnAdd);
        ThemeManager.StyleGhostButton(_btnRefresh);
        ThemeManager.StyleGhostButton(_btnEdit);
        ThemeManager.StyleGhostButton(_btnBackup);
        ThemeManager.StyleGhostButton(_btnImport);
        ThemeManager.StyleDangerButton(_btnDelete);
        ThemeManager.EnableFadeIn(this);
    }

    private void BuildStatsView()
    {
        _statsView.SetBounds(0, 0, 1078, 650);
        _statsView.BackColor = Color.Transparent;

        var header = new Panel { Left = 18, Top = 18, Width = 1042, Height = 64, BackColor = Color.Transparent };
        header.Paint += (_, e) => ThemeManager.DrawGlassPanel(e.Graphics, new Rectangle(0, 0, header.Width - 1, header.Height - 1), 18);
        var lblTitle = new Label { Text = "Аналитика безопасности", Left = 18, Top = 12, Width = 340, Font = ThemeManager.CreateFont(15F, FontStyle.Bold) };
        var lblHint = new Label { Text = "Графики по категориям, возрасту паролей и обнаруженным рискам.", Left = 18, Top = 36, Width = 470, ForeColor = ThemeManager.TextMuted, Font = ThemeManager.CreateFont(10.2F) };
        header.Controls.AddRange([lblTitle, lblHint]);

        _statsCardsHost.SetBounds(18, 96, 1042, 94);
        _statsCardsHost.BackColor = Color.Transparent;
        BuildMetricCard(_cardTotal, _cardTotalValue, "Всего записей", 0, ThemeManager.Accent);
        BuildMetricCard(_cardFav, _cardFavValue, "Избранные", 264, ThemeManager.Accent2);
        BuildMetricCard(_cardOld, _cardOldValue, "К смене", 528, ThemeManager.Warning);
        BuildMetricCard(_cardDuplicate, _cardDuplicateValue, "Повторы", 792, ThemeManager.Danger);
        _statsCardsHost.Controls.AddRange([_cardTotal, _cardFav, _cardOld, _cardDuplicate]);

        _categoryChartPanel.SetBounds(18, 208, 500, 416);
        _categoryChartPanel.BackColor = Color.Transparent;
        _categoryChartPanel.Paint += (_, e) => DrawCategoryChart(e.Graphics, _categoryChartPanel.ClientRectangle);

        _ageChartPanel.SetBounds(536, 208, 524, 200);
        _ageChartPanel.BackColor = Color.Transparent;
        _ageChartPanel.Paint += (_, e) => DrawAgeChart(e.Graphics, _ageChartPanel.ClientRectangle);

        _alertsPanel.SetBounds(536, 424, 524, 200);
        _alertsPanel.BackColor = Color.Transparent;
        _alertsPanel.Paint += (_, e) => ThemeManager.DrawGlassPanel(e.Graphics, new Rectangle(0, 0, _alertsPanel.Width - 1, _alertsPanel.Height - 1), 22);
        var alertsTitle = new Label { Text = "Обнаруженные риски", Left = 18, Top = 14, Width = 250, Font = ThemeManager.CreateFont(13F, FontStyle.Bold) };
        var alertsHint = new Label { Text = "Список сервисов, которые стоит проверить в первую очередь.", Left = 18, Top = 38, Width = 350, ForeColor = ThemeManager.TextMuted, Font = ThemeManager.CreateFont(9.8F) };
        _lstAlerts.SetBounds(18, 70, 488, 112);
        _alertsPanel.Controls.AddRange([alertsTitle, alertsHint, _lstAlerts]);

        _statsView.Controls.AddRange([header, _statsCardsHost, _categoryChartPanel, _ageChartPanel, _alertsPanel]);
    }

    private void BuildMetricCard(Panel panel, Label valueLabel, string title, int left, Color accent)
    {
        panel.SetBounds(left, 0, 250, 94);
        panel.BackColor = Color.Transparent;
        panel.Paint += (_, e) => DrawMetricCard(e.Graphics, panel.ClientRectangle, accent);

        var titleLabel = new Label
        {
            Text = title,
            Left = 18,
            Top = 14,
            Width = 150,
            ForeColor = ThemeManager.TextMuted,
            Font = ThemeManager.CreateFont(10F, FontStyle.Bold)
        };

        valueLabel.Left = 18;
        valueLabel.Top = 40;
        valueLabel.Width = 140;
        valueLabel.Height = 34;
        valueLabel.Font = ThemeManager.CreateFont(20F, FontStyle.Bold);

        panel.Controls.AddRange([titleLabel, valueLabel]);
    }

    private void DrawMetricCard(Graphics g, Rectangle bounds, Color accent)
    {
        var rect = new Rectangle(0, 0, bounds.Width - 1, bounds.Height - 1);
        ThemeManager.DrawGlassPanel(g, rect, 20);
        using var brush = new LinearGradientBrush(new Rectangle(0, 0, 8, bounds.Height - 1), accent, ControlPaint.Light(accent, 0.2f), 90F);
        using var path = ThemeManager.CreateRoundedPath(new Rectangle(0, 0, 8, bounds.Height - 1), 6);
        g.FillPath(brush, path);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        ThemeManager.DrawGlow(e.Graphics, new Rectangle(-70, -24, 300, 170));
        ThemeManager.DrawGlow(e.Graphics, new Rectangle(1130, -10, 220, 130));
        ThemeManager.DrawGlow(e.Graphics, new Rectangle(1090, 700, 260, 150));
    }

    private void PrepareNavButton(Button button, string text, int top)
    {
        button.Text = text;
        button.SetBounds(20, top, 196, 54);
    }

    private void PrepareActionButton(Button button, string text, int left, int top, int width)
    {
        button.Text = text;
        button.SetBounds(left, top, width, 34);
    }

    private void PrepareGridView(DataGridView grid)
    {
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.MultiSelect = false;
        grid.AutoGenerateColumns = false;
        grid.RowTemplate.Height = 38;
        grid.Columns.Clear();
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Сервис", DataPropertyName = nameof(PasswordEntry.ServiceName), FillWeight = 22 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Логин", DataPropertyName = nameof(PasswordEntry.Login), FillWeight = 20 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Пароль", DataPropertyName = nameof(PasswordEntry.Password), FillWeight = 16 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Категория", DataPropertyName = nameof(PasswordEntry.Category), FillWeight = 12 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Создан", DataPropertyName = nameof(PasswordEntry.CreatedAt), FillWeight = 10, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy" } });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Изменён", DataPropertyName = nameof(PasswordEntry.LastChangedAt), FillWeight = 10, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy" } });
        grid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "★", DataPropertyName = nameof(PasswordEntry.IsFavorite), FillWeight = 5 });
    }

    private void ShowSection(string section)
    {
        _allView.Visible = section == "all";
        _favView.Visible = section == "fav";
        _statsView.Visible = section == "stats";

        ThemeManager.StyleNavButton(_btnNavAll, section == "all");
        ThemeManager.StyleNavButton(_btnNavFav, section == "fav");
        ThemeManager.StyleNavButton(_btnNavStats, section == "stats");

        var targetTop = section == "all" ? _btnNavAll.Top + 4 : section == "fav" ? _btnNavFav.Top + 4 : _btnNavStats.Top + 4;
        _navIndicator.Top = targetTop;
        _navIndicator.Height = 40;
        _navIndicator.BringToFront();
    }

    private void LoadAll()
    {
        RefreshGrid();
        RefreshFavorites();
        RefreshStats();
        ShowAlertsPopup();
    }

    private void RefreshGrid()
    {
        _gridAll.DataSource = null;
        _gridAll.DataSource = _vaultService.Search(_txtSearch.Text, _cmbCategory.SelectedItem?.ToString());
        UpdateSummary();
    }

    private void RefreshFavorites()
    {
        _gridFav.DataSource = null;
        _gridFav.DataSource = _vaultService.Vault.Entries.Where(x => x.IsFavorite).OrderBy(x => x.ServiceName).ToList();
    }

    private void RefreshStats()
    {
        _categoryStats = _vaultService.CategoryStats();
        _oldPasswords = _vaultService.GetOldPasswords();
        _duplicateGroups = _vaultService.GetDuplicatePasswordGroups();

        _cardTotalValue.Text = _vaultService.Vault.Entries.Count.ToString();
        _cardFavValue.Text = _vaultService.Vault.Entries.Count(x => x.IsFavorite).ToString();
        _cardOldValue.Text = _oldPasswords.Count.ToString();
        _cardDuplicateValue.Text = _duplicateGroups.Count.ToString();

        _lstAlerts.Items.Clear();
        if (_oldPasswords.Count == 0 && _duplicateGroups.Count == 0)
            _lstAlerts.Items.Add("Риски не найдены. Сейф выглядит надёжно.");

        foreach (var old in _oldPasswords.Take(12))
            _lstAlerts.Items.Add($"⚠ Не менялся больше полугода: {old.ServiceName}");

        foreach (var group in _duplicateGroups.Take(8))
        {
            var services = string.Join(", ", group.Select(x => x.ServiceName));
            _lstAlerts.Items.Add($"⚠ Повторяется в сервисах: {services}");
        }

        _categoryChartPanel.Invalidate();
        _ageChartPanel.Invalidate();
    }

    private void UpdateSummary()
    {
        var total = _vaultService.Vault.Entries.Count;
        var fav = _vaultService.Vault.Entries.Count(x => x.IsFavorite);
        var old = _vaultService.GetOldPasswords().Count;
        _lblSummary.Text = $"Всего {total}  •  Избранных {fav}  •  К смене {old}";
    }

    private void DrawCategoryChart(Graphics g, Rectangle bounds)
    {
        ThemeManager.DrawGlassPanel(g, new Rectangle(0, 0, bounds.Width - 1, bounds.Height - 1), 22);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var titleBrush = new SolidBrush(ThemeManager.TextPrimary);
        using var mutedBrush = new SolidBrush(ThemeManager.TextMuted);
        g.DrawString("Пароли по категориям", ThemeManager.CreateFont(13.5F, FontStyle.Bold), titleBrush, 18, 14);
        g.DrawString("Сколько записей хранится в личном, рабочем и финансовом блоках.", ThemeManager.CreateFont(9.8F), mutedBrush, 18, 38);

        var chartRect = new Rectangle(26, 86, bounds.Width - 52, bounds.Height - 126);
        using var axisPen = new Pen(Color.FromArgb(85, ThemeManager.Border), 1F);
        for (int i = 0; i <= 4; i++)
        {
            var y = chartRect.Top + (chartRect.Height * i / 4);
            g.DrawLine(axisPen, chartRect.Left, y, chartRect.Right, y);
        }

        var data = new[]
        {
            (Name: "Личное", Value: GetCategoryValue("Личное"), Color1: ThemeManager.Accent, Color2: ControlPaint.Light(ThemeManager.Accent, 0.15f)),
            (Name: "Работа", Value: GetCategoryValue("Работа"), Color1: ThemeManager.Accent2, Color2: ControlPaint.Light(ThemeManager.Accent2, 0.15f)),
            (Name: "Финансы", Value: GetCategoryValue("Финансы"), Color1: ThemeManager.Warning, Color2: ControlPaint.Light(ThemeManager.Warning, 0.10f))
        };

        var max = Math.Max(1, data.Max(x => x.Value));
        var step = chartRect.Width / data.Length;
        var barWidth = 76;

        for (int i = 0; i < data.Length; i++)
        {
            var item = data[i];
            var x = chartRect.Left + i * step + (step - barWidth) / 2;
            var height = (int)((chartRect.Height - 20) * (item.Value / (double)max));
            var barRect = new Rectangle(x, chartRect.Bottom - height, barWidth, Math.Max(14, height));

            using var path = ThemeManager.CreateRoundedPath(barRect, 10);
            using var brush = new LinearGradientBrush(barRect, item.Color1, item.Color2, 90F);
            g.FillPath(brush, path);
            using var pen = new Pen(Color.FromArgb(100, Color.White), 1F);
            g.DrawPath(pen, path);

            var valueText = item.Value.ToString();
            var valueSize = g.MeasureString(valueText, ThemeManager.CreateFont(11F, FontStyle.Bold));
            g.DrawString(valueText, ThemeManager.CreateFont(11F, FontStyle.Bold), titleBrush, x + (barWidth - valueSize.Width) / 2, barRect.Top - 26);

            var labelSize = g.MeasureString(item.Name, ThemeManager.CreateFont(10F, FontStyle.Bold));
            g.DrawString(item.Name, ThemeManager.CreateFont(10F, FontStyle.Bold), mutedBrush, x + (barWidth - labelSize.Width) / 2, chartRect.Bottom + 10);
        }
    }

    private int GetCategoryValue(string name)
    {
        foreach (var pair in _categoryStats)
            if (string.Equals(pair.Key, name, StringComparison.OrdinalIgnoreCase))
                return pair.Value;
        return 0;
    }

    private void DrawAgeChart(Graphics g, Rectangle bounds)
    {
        ThemeManager.DrawGlassPanel(g, new Rectangle(0, 0, bounds.Width - 1, bounds.Height - 1), 22);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        using var titleBrush = new SolidBrush(ThemeManager.TextPrimary);
        using var mutedBrush = new SolidBrush(ThemeManager.TextMuted);
        g.DrawString("Возраст паролей", ThemeManager.CreateFont(13.5F, FontStyle.Bold), titleBrush, 18, 14);
        g.DrawString("Чем старше пароль, тем выше риск — это видно на графике ниже.", ThemeManager.CreateFont(9.8F), mutedBrush, 18, 38);

        var now = DateTime.Now;
        var buckets = new[]
        {
            (Label: "< 3 мес", Value: _vaultService.Vault.Entries.Count(x => (now - x.LastChangedAt).TotalDays < 90), Color: ThemeManager.Success),
            (Label: "3-6 мес", Value: _vaultService.Vault.Entries.Count(x => (now - x.LastChangedAt).TotalDays >= 90 && (now - x.LastChangedAt).TotalDays < 180), Color: ThemeManager.Accent2),
            (Label: "6-12 мес", Value: _vaultService.Vault.Entries.Count(x => (now - x.LastChangedAt).TotalDays >= 180 && (now - x.LastChangedAt).TotalDays < 365), Color: ThemeManager.Warning),
            (Label: "> 12 мес", Value: _vaultService.Vault.Entries.Count(x => (now - x.LastChangedAt).TotalDays >= 365), Color: ThemeManager.Danger)
        };

        var max = Math.Max(1, buckets.Max(x => x.Value));
        var startY = 84;
        var fullWidth = bounds.Width - 160;

        for (int i = 0; i < buckets.Length; i++)
        {
            var item = buckets[i];
            var y = startY + i * 28;
            g.DrawString(item.Label, ThemeManager.CreateFont(10F, FontStyle.Bold), mutedBrush, 18, y + 4);

            var trackRect = new Rectangle(112, y, fullWidth, 16);
            using var trackPath = ThemeManager.CreateRoundedPath(trackRect, 8);
            using var trackBrush = new SolidBrush(Color.FromArgb(55, ThemeManager.Border));
            g.FillPath(trackBrush, trackPath);

            var fillWidth = Math.Max(12, (int)(fullWidth * (item.Value / (double)max)));
            var fillRect = new Rectangle(112, y, fillWidth, 16);
            using var fillPath = ThemeManager.CreateRoundedPath(fillRect, 8);
            using var fillBrush = new LinearGradientBrush(fillRect, item.Color, ControlPaint.Light(item.Color, 0.10f), 0F);
            g.FillPath(fillBrush, fillPath);

            g.DrawString(item.Value.ToString(), ThemeManager.CreateFont(10F, FontStyle.Bold), titleBrush, trackRect.Right + 10, y + 1);
        }
    }

    private PasswordEntry? GetSelectedEntry()
    {
        if (_favView.Visible)
            return _gridFav.CurrentRow?.DataBoundItem as PasswordEntry;
        return _gridAll.CurrentRow?.DataBoundItem as PasswordEntry;
    }

    private void AddEntry()
    {
        using var dialog = new EntryEditorForm();
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        _vaultService.Vault.Entries.Add(dialog.Entry);
        _vaultService.Save();
        LoadAll();
    }

    private void EditSelected()
    {
        var selected = GetSelectedEntry();
        if (selected is null)
        {
            MessageBox.Show("Сначала выберите запись.", "MaxVault", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new EntryEditorForm(selected);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        var index = _vaultService.Vault.Entries.FindIndex(x => x.Id == selected.Id);
        if (index >= 0)
        {
            _vaultService.Vault.Entries[index] = dialog.Entry;
            _vaultService.Save();
            LoadAll();
        }
    }

    private void DeleteSelected()
    {
        var selected = GetSelectedEntry();
        if (selected is null)
        {
            MessageBox.Show("Сначала выберите запись.", "MaxVault", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show($"Удалить '{selected.ServiceName}'?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        _vaultService.Vault.Entries.RemoveAll(x => x.Id == selected.Id);
        _vaultService.Save();
        LoadAll();
    }

    private void ExportBackup()
    {
        using var dialog = new FolderBrowserDialog { Description = "Папка для резервной копии" };
        if (dialog.ShowDialog() != DialogResult.OK) return;

        var backupPath = _vaultService.ExportEncryptedBackup(dialog.SelectedPath);
        MessageBox.Show($"Резервная копия сохранена:\n{backupPath}", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ImportBackup()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Encrypted backup (*.bak)|*.bak|All files (*.*)|*.*",
            Title = "Выберите резервную копию"
        };

        if (dialog.ShowDialog() != DialogResult.OK) return;

        if (MessageBox.Show("Импорт заменит текущий сейф. Продолжить?", "Импорт", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        try
        {
            _vaultService.ImportEncryptedBackup(dialog.FileName);
            MessageBox.Show("Файл импортирован. Перезапустите приложение.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка импорта", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowAlertsPopup()
    {
        var old = _vaultService.GetOldPasswords();
        var duplicates = _vaultService.GetDuplicatePasswordGroups();
        if (old.Count == 0 && duplicates.Count == 0) return;

        var builder = new StringBuilder();
        if (old.Count > 0)
            builder.AppendLine($"• Старые пароли: {old.Count}");
        if (duplicates.Count > 0)
            builder.AppendLine($"• Повторы: {duplicates.Count}");

        MessageBox.Show(
            $"Обнаружены риски безопасности:\n\n{builder}\nПодробнее — в разделе 'Статистика'.",
            "Проверка безопасности",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }
}
