using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LabAnalysisUI.Helpers;

public static class ThemeHelper
{
    public static Color AppBackground => Color.FromArgb(244, 247, 253);
    public static Color HeaderBackground => Color.FromArgb(232, 239, 255);
    public static Color Surface => Color.FromArgb(255, 255, 255);
    public static Color SurfaceAlt => Color.FromArgb(247, 250, 255);
    public static Color ResultBackground => Color.FromArgb(250, 252, 255);
    public static Color Border => Color.FromArgb(220, 228, 246);
    public static Color Accent => Color.FromArgb(41, 150, 240);
    public static Color AccentSecondary => Color.FromArgb(41, 87, 240);
    public static Color AccentTertiary => Color.FromArgb(41, 213, 240);
    public static Color AccentPurple => Color.FromArgb(123, 41, 240);
    public static Color AccentSoft => Color.FromArgb(138, 163, 244);
    public static Color AccentDark => Color.FromArgb(28, 72, 179);
    public static Color TextPrimary => Color.FromArgb(28, 35, 52);
    public static Color TextMuted => Color.FromArgb(101, 113, 144);
    public static Color Success => Color.FromArgb(34, 139, 110);
    public static Color Warning => Color.FromArgb(180, 118, 28);
    public static Color Danger => Color.FromArgb(191, 67, 92);

    public static void ConfigureForm(Form form)
    {
        form.BackColor = AppBackground;
        form.ForeColor = TextPrimary;
        form.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
    }

    public static void ConfigureTabs(TabControl tabs)
    {
        tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
        tabs.ItemSize = new Size(260, 42);
        tabs.SizeMode = TabSizeMode.Fixed;
        tabs.Padding = new Point(16, 6);
        tabs.DrawItem -= DrawTab;
        tabs.DrawItem += DrawTab;
    }

    public static void StyleCard(Control control)
    {
        control.BackColor = Surface;
        control.ForeColor = TextPrimary;
        control.Paint -= DrawCardBorder;
        control.Paint += DrawCardBorder;
    }

    public static void StylePrimaryButton(Button button)
    {
        ApplyButtonStyle(button, Accent, AccentSecondary, Color.White, 0, Accent);
    }

    public static void StyleSecondaryButton(Button button)
    {
        ApplyButtonStyle(button, SurfaceAlt, Color.FromArgb(237, 242, 255), TextPrimary, 1, AccentSoft);
    }

    public static void StyleTextBox(TextBox textBox)
    {
        textBox.BackColor = ResultBackground;
        textBox.ForeColor = TextPrimary;
        textBox.BorderStyle = BorderStyle.FixedSingle;
        textBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
    }

    public static void StyleNumeric(NumericUpDown numeric)
    {
        numeric.BackColor = ResultBackground;
        numeric.ForeColor = TextPrimary;
        numeric.BorderStyle = BorderStyle.FixedSingle;
        numeric.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
    }

    public static void StyleResultBox(RichTextBox box)
    {
        box.BackColor = ResultBackground;
        box.ForeColor = TextPrimary;
        box.BorderStyle = BorderStyle.FixedSingle;
        box.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
    }

    public static void StyleCheckBox(CheckBox checkBox)
    {
        checkBox.ForeColor = TextMuted;
        checkBox.BackColor = Color.Transparent;
        checkBox.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
    }

    public static void StyleStatusLabel(Label label, Color accent)
    {
        label.AutoSize = true;
        label.ForeColor = accent;
        label.BackColor = Color.FromArgb(28, accent);
        label.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
        label.Padding = new Padding(10, 6, 10, 6);
    }

    private static void ApplyButtonStyle(Button button, Color baseColor, Color hoverColor, Color textColor, int borderSize, Color borderColor)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = borderSize;
        if (borderSize > 0)
        {
            button.FlatAppearance.BorderColor = borderColor;
        }

        button.BackColor = baseColor;
        button.ForeColor = textColor;
        button.Cursor = Cursors.Hand;
        button.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);

        button.MouseEnter += (_, _) =>
        {
            if (button.Enabled)
            {
                button.BackColor = hoverColor;
            }
        };

        button.MouseLeave += (_, _) =>
        {
            if (button.Enabled)
            {
                button.BackColor = baseColor;
            }
        };

        button.EnabledChanged += (_, _) =>
        {
            if (button.Enabled)
            {
                button.BackColor = baseColor;
                button.ForeColor = textColor;
                if (borderSize > 0)
                {
                    button.FlatAppearance.BorderColor = borderColor;
                }
            }
            else
            {
                button.BackColor = Color.FromArgb(236, 240, 247);
                button.ForeColor = Color.FromArgb(153, 163, 184);
                if (borderSize > 0)
                {
                    button.FlatAppearance.BorderColor = Border;
                }
            }
        };
    }

    private static void DrawCardBorder(object? sender, PaintEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }

        var rectangle = new Rectangle(0, 0, control.Width - 1, control.Height - 1);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Border);
        e.Graphics.DrawRectangle(pen, rectangle);
    }

    private static void DrawTab(object? sender, DrawItemEventArgs e)
    {
        if (sender is not TabControl tabs)
        {
            return;
        }

        var tabBounds = Rectangle.Inflate(e.Bounds, -6, -4);
        var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        using var borderPen = new Pen(isSelected ? Accent : Border);
        using var path = CreateRoundedRectangle(tabBounds, 12);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        if (isSelected)
        {
            using var gradient = new LinearGradientBrush(tabBounds, Accent, AccentSoft, LinearGradientMode.Horizontal);
            var blend = new ColorBlend
            {
                Colors = new[] { Accent, AccentSecondary, AccentTertiary, AccentPurple },
                Positions = new[] { 0F, 0.32F, 0.7F, 1F }
            };
            gradient.InterpolationColors = blend;
            e.Graphics.FillPath(gradient, path);
        }
        else
        {
            using var fillBrush = new SolidBrush(Surface);
            e.Graphics.FillPath(fillBrush, path);
        }

        e.Graphics.DrawPath(borderPen, path);

        TextRenderer.DrawText(
            e.Graphics,
            tabs.TabPages[e.Index].Text,
            new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold, GraphicsUnit.Point),
            tabBounds,
            isSelected ? Color.White : TextMuted,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle rectangle, int radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;

        path.AddArc(rectangle.X, rectangle.Y, diameter, diameter, 180, 90);
        path.AddArc(rectangle.Right - diameter, rectangle.Y, diameter, diameter, 270, 90);
        path.AddArc(rectangle.Right - diameter, rectangle.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rectangle.X, rectangle.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}
