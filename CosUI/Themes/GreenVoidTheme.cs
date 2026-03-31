using System.Windows.Media;

namespace CosUI.Themes;

public sealed class GreenVoidTheme : ThemeBase
{
    public override string Name => "Green Void";

    public override Color Background    => Hex("#070f09");
    public override Color Surface       => Hex("#0d220f");
    public override Color SidebarBg     => Hex("#080f0a");
    public override Color BorderSubtle  => Hex("#111a14");
    public override Color BorderActive  => Hex("#1a3820");
    public override Color PrimaryBlue   => Hex("#1aff66");
    public override Color AccentBlue    => Hex("#66ff99");
    public override Color TextPrimary   => Hex("#d0ffd8");
    public override Color TextSecondary => Hex("#7ae09a");
    public override Color TextMuted     => Hex("#4a6a55");
    public override Color Error         => Hex("#cc4444");
    public override Color Warn          => Hex("#bb8822");
    public override Color StatusOn      => Hex("#33ee66");
    public override Color StatusOnRing  => Hex("#22cc55");
    public override Color StatusOff     => Hex("#cc3333");
    public override Color TextPrint     => Hex("#44bb66");
    public override Color TextInfo      => Hex("#338855");
    public override Color TextWarn      => Hex("#c09030");
    public override Color TextError     => Hex("#cc5555");

    public override Color GlowPrimary         => Hex("#1aff66");
    public override double GlowPrimaryBlur    => 16;
    public override double GlowPrimaryOpacity => 0.35;
    public override Color GlowLogo            => Hex("#1aff6f");
    public override double GlowLogoBlur       => 12;

    public override Color WinClose    => Hex("#FF5F57");
    public override Color WinMinimize => Hex("#FEBC2E");
    public override Color WinMaximize => Hex("#28C840");
    public override Color Shadow      => Hex("#000000");

    public override string EditorFontFamily => "Cascadia Code, Consolas, monospace";
    public override double EditorFontSize   => 13;
    public override double EditorLineHeight => 1.75;
}
