using System.Windows.Media;

namespace CosUI.Themes;

public sealed class RedVoidTheme : ThemeBase
{
    public override string Name => "Red Void";

    public override Color Background    => Hex("#0f0707");
    public override Color Surface       => Hex("#220d0d");
    public override Color SidebarBg     => Hex("#150808");
    public override Color BorderSubtle  => Hex("#381111");
    public override Color BorderActive  => Hex("#551a1a");
    public override Color PrimaryBlue   => Hex("#ff1a1a");
    public override Color AccentBlue    => Hex("#ff6666");
    public override Color TextPrimary   => Hex("#ffd0d0");
    public override Color TextSecondary => Hex("#e07a7a");
    public override Color TextMuted     => Hex("#6a4a4a");
    public override Color Error         => Hex("#cc4444");
    public override Color Warn          => Hex("#bb8822");
    public override Color StatusOn      => Hex("#33ee66");
    public override Color StatusOnRing  => Hex("#22cc55");
    public override Color StatusOff     => Hex("#cc3333");
    public override Color TextPrint     => Hex("#dd6655");
    public override Color TextInfo      => Hex("#bb4444");
    public override Color TextWarn      => Hex("#c09030");
    public override Color TextError     => Hex("#cc5555");

    public override Color GlowPrimary         => Hex("#ff1a1a");
    public override double GlowPrimaryBlur    => 16;
    public override double GlowPrimaryOpacity => 0.35;
    public override Color GlowLogo            => Hex("#ff2a2a");
    public override double GlowLogoBlur       => 12;

    public override Color WinClose    => Hex("#FF5F57");
    public override Color WinMinimize => Hex("#FEBC2E");
    public override Color WinMaximize => Hex("#28C840");
    public override Color Shadow      => Hex("#000000");

    public override string EditorFontFamily => "Cascadia Code, Consolas, monospace";
    public override double EditorFontSize   => 13;
    public override double EditorLineHeight => 1.75;
}
