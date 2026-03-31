using System.Windows.Media;

namespace CosUI.Themes;

public sealed class PurpleVoidTheme : ThemeBase
{
    public override string Name => "Purple Void";

    public override Color Background    => Hex("#09070f");
    public override Color Surface       => Hex("#190d22");
    public override Color SidebarBg     => Hex("#0b0815");
    public override Color BorderSubtle  => Hex("#1a1138");
    public override Color BorderActive  => Hex("#2a1a55");
    public override Color PrimaryBlue   => Hex("#7a1aff");
    public override Color AccentBlue    => Hex("#aa66ff");
    public override Color TextPrimary   => Hex("#e8d0ff");
    public override Color TextSecondary => Hex("#9d7ae0");
    public override Color TextMuted     => Hex("#5a4a6a");
    public override Color Error         => Hex("#cc4444");
    public override Color Warn          => Hex("#bb8822");
    public override Color StatusOn      => Hex("#33ee66");
    public override Color StatusOnRing  => Hex("#22cc55");
    public override Color StatusOff     => Hex("#cc3333");
    public override Color TextPrint     => Hex("#8866dd");
    public override Color TextInfo      => Hex("#6644bb");
    public override Color TextWarn      => Hex("#c09030");
    public override Color TextError     => Hex("#cc5555");

    public override Color GlowPrimary         => Hex("#7a1aff");
    public override double GlowPrimaryBlur    => 16;
    public override double GlowPrimaryOpacity => 0.35;
    public override Color GlowLogo            => Hex("#8a2aff");
    public override double GlowLogoBlur       => 12;

    public override Color WinClose    => Hex("#FF5F57");
    public override Color WinMinimize => Hex("#FEBC2E");
    public override Color WinMaximize => Hex("#28C840");
    public override Color Shadow      => Hex("#000000");

    public override string EditorFontFamily => "Cascadia Code, Consolas, monospace";
    public override double EditorFontSize   => 13;
    public override double EditorLineHeight => 1.75;
}
