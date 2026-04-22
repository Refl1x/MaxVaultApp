using System.Drawing.Drawing2D;
using System.Linq;

namespace MaxVaultApp.UI;

public static class ThemeManager
{
    public static readonly Color Background = Color.FromArgb(10, 14, 28);
    public static readonly Color BackgroundGlow = Color.FromArgb(24, 34, 66);
    public static readonly Color Surface = Color.FromArgb(18, 24, 42);
    public static readonly Color SurfaceAlt = Color.FromArgb(25, 33, 55);
    public static readonly Color SurfaceSoft = Color.FromArgb(31, 42, 70);
    public static readonly Color SurfaceGlass = Color.FromArgb(22, 29, 49);
    public static readonly Color Border = Color.FromArgb(71, 88, 132);
    public static readonly Color Accent = Color.FromArgb(122, 114, 255);
    public static readonly Color Accent2 = Color.FromArgb(90, 212, 255);
    public static readonly Color AccentSoft = Color.FromArgb(52, 65, 111);
    public static readonly Color AccentSoft2 = Color.FromArgb(47, 85, 121);
    public static readonly Color TextPrimary = Color.FromArgb(241, 245, 255);
    public static readonly Color TextMuted = Color.FromArgb(160, 172, 204);
    public static readonly Color Danger = Color.FromArgb(255, 108, 141);
    public static readonly Color Success = Color.FromArgb(65, 205, 156);
    public static readonly Color Warning = Color.FromArgb(255, 189, 92);

    private static readonly string[] PreferredFonts =
    [
        "Segoe UI Variable Display",
        "Segoe UI Variable Text",
        "Segoe UI",
        "Bahnschrift",
        "Trebuchet MS",
        "Arial"
    ];

    public static string FontFamilyName => FontFamily.Families
        .Select(f => f.Name)
        .FirstOrDefault(name => PreferredFonts.Contains(name, StringComparer.OrdinalIgnoreCase))
        ?? "Segoe UI";

    public static Font CreateFont(float size, FontStyle style = FontStyle.Regular)
        => new(FontFamilyName, size, style);

    public static void ApplyTheme(Control root)
    {
        root.BackColor = Background;
        root.ForeColor = TextPrimary;

        foreach (Control control in root.Controls)
        {
            StyleControl(control);
            if (control.HasChildren)
                ApplyTheme(control);
        }
    }

    private static void StyleControl(Control control)
    {
        switch (control)
        {
            case Button button:
                button.BackColor = Accent;
                button.ForeColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.Font = CreateFont(9.6F, FontStyle.Bold);
                button.Cursor = Cursors.Hand;
                button.TextAlign = ContentAlignment.MiddleCenter;
                button.UseCompatibleTextRendering = true;
                button.AutoSize = false;
                button.Padding = new Padding(6, 0, 6, 0);
                break;
            case TextBox textBox:
                textBox.BackColor = SurfaceAlt;
                textBox.ForeColor = TextPrimary;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Font = CreateFont(10.6F);
                break;
            case ComboBox comboBox:
                comboBox.BackColor = SurfaceAlt;
                comboBox.ForeColor = TextPrimary;
                comboBox.FlatStyle = FlatStyle.Flat;
                comboBox.Font = CreateFont(10.4F);
                break;
            case NumericUpDown numeric:
                numeric.BackColor = SurfaceAlt;
                numeric.ForeColor = TextPrimary;
                numeric.BorderStyle = BorderStyle.FixedSingle;
                numeric.Font = CreateFont(10.4F);
                break;
            case DateTimePicker picker:
                picker.CalendarMonthBackground = Surface;
                picker.CalendarForeColor = TextPrimary;
                picker.CalendarTitleBackColor = Accent;
                picker.CalendarTitleForeColor = Color.White;
                picker.CalendarTrailingForeColor = TextMuted;
                picker.Font = CreateFont(10.4F);
                break;
            case DataGridView grid:
                StyleGrid(grid);
                break;
            case Panel panel:
                panel.BackColor = Surface;
                break;
            case GroupBox groupBox:
                groupBox.ForeColor = TextPrimary;
                groupBox.BackColor = Surface;
                groupBox.Font = CreateFont(10.4F, FontStyle.Bold);
                break;
            case ListBox listBox:
                listBox.BackColor = SurfaceAlt;
                listBox.ForeColor = TextPrimary;
                listBox.BorderStyle = BorderStyle.None;
                listBox.Font = CreateFont(10.2F);
                break;
            case CheckBox checkBox:
                checkBox.ForeColor = TextPrimary;
                checkBox.BackColor = Color.Transparent;
                checkBox.Font = CreateFont(10.2F);
                break;
            case Label label:
                label.ForeColor = TextPrimary;
                label.BackColor = Color.Transparent;
                if (label.Font.FontFamily.Name == SystemFonts.DefaultFont.FontFamily.Name)
                    label.Font = CreateFont(label.Font.Size, label.Font.Style);
                break;
        }
    }

    private static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.EnableHeadersVisualStyles = false;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.ColumnHeadersDefaultCellStyle.BackColor = SurfaceAlt;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
        grid.ColumnHeadersDefaultCellStyle.Font = CreateFont(9.6F, FontStyle.Bold);
        grid.ColumnHeadersHeight = 42;
        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = TextPrimary;
        grid.DefaultCellStyle.SelectionBackColor = AccentSoft;
        grid.DefaultCellStyle.SelectionForeColor = TextPrimary;
        grid.DefaultCellStyle.Padding = new Padding(6);
        grid.DefaultCellStyle.Font = CreateFont(10F);
        grid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(22, 29, 49);
        grid.AlternatingRowsDefaultCellStyle.ForeColor = TextPrimary;
        grid.GridColor = Border;
    }

    public static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }

    public static void RoundControl(Control control, int radius = 16)
    {
        if (control.Width <= 0 || control.Height <= 0) return;
        using var path = CreateRoundedPath(new Rectangle(0, 0, control.Width, control.Height), radius);
        control.Region = new Region(path);
    }

    public static void DrawPanel(Graphics g, Rectangle bounds, int radius = 22)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = CreateRoundedPath(bounds, radius);
        using var brush = new LinearGradientBrush(bounds, Color.FromArgb(230, Surface), Color.FromArgb(240, SurfaceAlt), 35F);
        g.FillPath(brush, path);
        using var pen = new Pen(Color.FromArgb(95, Border), 1.1F);
        g.DrawPath(pen, path);
    }

    public static void DrawGlassPanel(Graphics g, Rectangle bounds, int radius = 22)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = CreateRoundedPath(bounds, radius);
        using var brush = new LinearGradientBrush(bounds, Color.FromArgb(232, SurfaceGlass), Color.FromArgb(242, SurfaceAlt), 90F);
        g.FillPath(brush, path);
        using var shine = new Pen(Color.FromArgb(40, Color.White), 1F);
        g.DrawPath(shine, path);
        using var pen = new Pen(Color.FromArgb(105, Border), 1F);
        g.DrawPath(pen, path);
    }

    public static void DrawGlow(Graphics g, Rectangle bounds)
    {
        using var glow = new LinearGradientBrush(bounds, Color.FromArgb(50, Accent), Color.FromArgb(20, Accent2), 0F);
        g.FillEllipse(glow, bounds);
    }

    public static void StylePrimaryButton(Button button)
    {
        button.BackColor = Accent;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Font = CreateFont(9.6F, FontStyle.Bold);
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.UseCompatibleTextRendering = true;
        button.Padding = new Padding(6, 0, 6, 0);
    }

    public static void StyleGhostButton(Button button)
    {
        button.BackColor = SurfaceAlt;
        button.ForeColor = TextPrimary;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Border;
        button.Font = CreateFont(9.6F, FontStyle.Bold);
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.UseCompatibleTextRendering = true;
        button.Padding = new Padding(6, 0, 6, 0);
    }

    public static void StyleDangerButton(Button button)
    {
        button.BackColor = Danger;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Font = CreateFont(9.6F, FontStyle.Bold);
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.UseCompatibleTextRendering = true;
        button.Padding = new Padding(6, 0, 6, 0);
    }

    public static void StyleNavButton(Button button, bool selected = false)
    {
        button.BackColor = selected ? Color.FromArgb(41, 53, 88) : Color.Transparent;
        button.ForeColor = selected ? Color.White : TextMuted;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Font = CreateFont(selected ? 12.8F : 12.0F, FontStyle.Bold);
        button.TextAlign = ContentAlignment.MiddleLeft;
        button.Padding = new Padding(18, 0, 0, 0);
        button.UseCompatibleTextRendering = true;
        button.Cursor = Cursors.Hand;
    }

    public static void EnableFadeIn(Form form)
    {
        form.Opacity = 0;
        var timer = new System.Windows.Forms.Timer { Interval = 15 };
        timer.Tick += (_, _) =>
        {
            form.Opacity = Math.Min(1, form.Opacity + 0.08);
            if (form.Opacity >= 1)
            {
                timer.Stop();
                timer.Dispose();
            }
        };
        form.Shown += (_, _) => timer.Start();
    }
}
