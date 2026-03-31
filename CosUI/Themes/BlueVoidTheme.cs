using System.Windows.Media;

namespace CosUI.Themes;

public sealed class BlueVoidTheme : ThemeBase
{
    public override string Name => "Blue Void";

    public override Color Background    => Hex("#07070f");
    public override Color Surface       => Hex("#0d0d22");
    public override Color SidebarBg     => Hex("#080815");
    public override Color BorderSubtle  => Hex("#111a38");
    public override Color BorderActive  => Hex("#1a2a55");
    public override Color PrimaryBlue   => Hex("#1a66ff");
    public override Color AccentBlue    => Hex("#6699ff");
    public override Color TextPrimary   => Hex("#d0e0ff");
    public override Color TextSecondary => Hex("#7a9de0");
    public override Color TextMuted     => Hex("#4a6aaa");
    public override Color Error         => Hex("#cc4444");
    public override Color Warn          => Hex("#bb8822");
    public override Color StatusOn      => Hex("#33ee66");
    public override Color StatusOnRing  => Hex("#22cc55");
    public override Color StatusOff     => Hex("#cc3333");
    public override Color TextPrint     => Hex("#6688dd");
    public override Color TextInfo      => Hex("#4477bb");
    public override Color TextWarn      => Hex("#c09030");
    public override Color TextError     => Hex("#cc5555");

    public override Color GlowPrimary         => Hex("#1a66ff");
    public override double GlowPrimaryBlur    => 16;
    public override double GlowPrimaryOpacity => 0.35;
    public override Color GlowLogo            => Hex("#1a6fff");
    public override double GlowLogoBlur       => 12;

    public override Color WinClose    => Hex("#FF5F57");
    public override Color WinMinimize => Hex("#FEBC2E");
    public override Color WinMaximize => Hex("#28C840");
    public override Color Shadow      => Hex("#000000");

    public override string EditorFontFamily => "Cascadia Code, Consolas, monospace";
    public override double EditorFontSize   => 13;
    public override double EditorLineHeight => 1.75;
}
