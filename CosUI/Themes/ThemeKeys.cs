namespace CosUI.Themes;

public static class ThemeKeys
{
    public const string Background      = nameof(Background);
    public const string Surface         = nameof(Surface);
    public const string SidebarBg       = nameof(SidebarBg);
    public const string BorderSubtle    = nameof(BorderSubtle);
    public const string BorderActive    = nameof(BorderActive);
    public const string PrimaryBlue     = nameof(PrimaryBlue);
    public const string AccentBlue      = nameof(AccentBlue);
    public const string TextPrimary     = nameof(TextPrimary);
    public const string TextSecondary   = nameof(TextSecondary);
    public const string TextMuted       = nameof(TextMuted);
    public const string Error           = nameof(Error);
    public const string Warn            = nameof(Warn);
    public const string StatusOn        = nameof(StatusOn);
    public const string StatusOnRing    = nameof(StatusOnRing);
    public const string StatusOff       = nameof(StatusOff);
    public const string TextPrint       = nameof(TextPrint);
    public const string TextInfo        = nameof(TextInfo);
    public const string TextWarn        = nameof(TextWarn);
    public const string TextError       = nameof(TextError);
    public const string GlowPrimary  = nameof(GlowPrimary);
    public const string GlowLogo     = nameof(GlowLogo);
    public const string WinClose     = nameof(WinClose);
    public const string WinMinimize  = nameof(WinMinimize);
    public const string WinMaximize  = nameof(WinMaximize);
    public const string Shadow       = nameof(Shadow);

    // Brush key = token key + "Brush" suffix
    public static string Brush(string key) => key + "Brush";
}
