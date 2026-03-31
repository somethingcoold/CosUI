using System.Windows.Media;

namespace CosUI.Themes;

public sealed class LightTheme : ThemeBase
{
    public override string Name => "Light";

    public override Color Background    => Hex("#f8fafc");
    public override Color Surface       => Hex("#ffffff");
    public override Color SidebarBg     => Hex("#f1f5f9");
    public override Color BorderSubtle  => Hex("#e2e8f0");
    public override Color BorderActive  => Hex("#cbd5e1");
    public override Color PrimaryBlue   => Hex("#2563eb");
    public override Color AccentBlue    => Hex("#3b82f6");
    public override Color TextPrimary   => Hex("#0f172a");
    public override Color TextSecondary => Hex("#334155");
    public override Color TextMuted     => Hex("#94a3b8");
    public override Color Error         => Hex("#dc2626");
    public override Color Warn          => Hex("#d97706");
    public override Color StatusOn      => Hex("#16a34a");
    public override Color StatusOnRing  => Hex("#15803d");
    public override Color StatusOff     => Hex("#dc2626");
    public override Color TextPrint     => Hex("#3b82f6");
    public override Color TextInfo      => Hex("#2563eb");
    public override Color TextWarn      => Hex("#d97706");
    public override Color TextError     => Hex("#dc2626");

    public override Color GlowPrimary         => Hex("#2563eb");
    public override double GlowPrimaryBlur    => 12;
    public override double GlowPrimaryOpacity => 0.20;
    public override Color GlowLogo            => Hex("#3b82f6");
    public override double GlowLogoBlur       => 8;

    public override Color WinClose    => Hex("#FF5F57");
    public override Color WinMinimize => Hex("#FEBC2E");
    public override Color WinMaximize => Hex("#28C840");
    public override Color Shadow      => Hex("#000000");

    public override string EditorFontFamily => "Cascadia Code, Consolas, monospace";
    public override double EditorFontSize   => 13;
    public override double EditorLineHeight => 1.75;
}
