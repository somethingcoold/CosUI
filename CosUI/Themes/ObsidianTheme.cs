using System.Windows.Media;

namespace CosUI.Themes;

public sealed class ObsidianTheme : ThemeBase
{
    public override string Name => "Obsidian";

    public override Color Background    => Hex("#080608");
    public override Color Surface       => Hex("#120d18");
    public override Color SidebarBg     => Hex("#0a080d");
    public override Color BorderSubtle  => Hex("#1c1524");
    public override Color BorderActive  => Hex("#2c1e3c");
    public override Color PrimaryBlue   => Hex("#9333ea");
    public override Color AccentBlue    => Hex("#c084fc");
    public override Color TextPrimary   => Hex("#f0eaff");
    public override Color TextSecondary => Hex("#c0a8e8");
    public override Color TextMuted     => Hex("#6a4a88");
    public override Color Error         => Hex("#cc4444");
    public override Color Warn          => Hex("#bb8822");
    public override Color StatusOn      => Hex("#33ee66");
    public override Color StatusOnRing  => Hex("#22cc55");
    public override Color StatusOff     => Hex("#cc3333");
    public override Color TextPrint     => Hex("#a07ccc");
    public override Color TextInfo      => Hex("#8855bb");
    public override Color TextWarn      => Hex("#c09030");
    public override Color TextError     => Hex("#cc5555");

    public override Color GlowPrimary         => Hex("#9333ea");
    public override double GlowPrimaryBlur    => 16;
    public override double GlowPrimaryOpacity => 0.35;
    public override Color GlowLogo            => Hex("#a855f7");
    public override double GlowLogoBlur       => 12;

    public override Color WinClose    => Hex("#FF5F57");
    public override Color WinMinimize => Hex("#FEBC2E");
    public override Color WinMaximize => Hex("#28C840");
    public override Color Shadow      => Hex("#000000");

    public override string EditorFontFamily => "Cascadia Code, Consolas, monospace";
    public override double EditorFontSize   => 13;
    public override double EditorLineHeight => 1.75;
}
