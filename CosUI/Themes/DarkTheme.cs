using System.Windows.Media;

namespace CosUI.Themes;

public sealed class DarkTheme : ThemeBase
{
    public override string Name => "Dark";

    public override Color Background    => Hex("#1e1e1e");
    public override Color Surface       => Hex("#252526");
    public override Color SidebarBg     => Hex("#1e1e1e");
    public override Color BorderSubtle  => Hex("#2d2d2d");
    public override Color BorderActive  => Hex("#3e3e42");
    public override Color PrimaryBlue   => Hex("#0ea5e9");
    public override Color AccentBlue    => Hex("#38bdf8");
    public override Color TextPrimary   => Hex("#d4d4d4");
    public override Color TextSecondary => Hex("#9d9d9d");
    public override Color TextMuted     => Hex("#636363");
    public override Color Error         => Hex("#f14c4c");
    public override Color Warn          => Hex("#cca700");
    public override Color StatusOn      => Hex("#4ec9b0");
    public override Color StatusOnRing  => Hex("#3bb29a");
    public override Color StatusOff     => Hex("#f14c4c");
    public override Color TextPrint     => Hex("#9cdcfe");
    public override Color TextInfo      => Hex("#4fc1ff");
    public override Color TextWarn      => Hex("#cca700");
    public override Color TextError     => Hex("#f14c4c");

    public override Color GlowPrimary         => Hex("#0ea5e9");
    public override double GlowPrimaryBlur    => 14;
    public override double GlowPrimaryOpacity => 0.30;
    public override Color GlowLogo            => Hex("#0ea5e9");
    public override double GlowLogoBlur       => 10;

    public override Color WinClose    => Hex("#FF5F57");
    public override Color WinMinimize => Hex("#FEBC2E");
    public override Color WinMaximize => Hex("#28C840");
    public override Color Shadow      => Hex("#000000");

    public override string EditorFontFamily => "Cascadia Code, Consolas, monospace";
    public override double EditorFontSize   => 13;
    public override double EditorLineHeight => 1.75;
}
