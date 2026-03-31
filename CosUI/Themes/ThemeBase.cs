using System.Windows.Media;

namespace CosUI.Themes;

public abstract class ThemeBase : ITheme
{
    public abstract string Name { get; }
    public abstract Color Background    { get; }
    public abstract Color Surface       { get; }
    public abstract Color SidebarBg     { get; }
    public abstract Color BorderSubtle  { get; }
    public abstract Color BorderActive  { get; }
    public abstract Color PrimaryBlue   { get; }
    public abstract Color AccentBlue    { get; }
    public abstract Color TextPrimary   { get; }
    public abstract Color TextSecondary { get; }
    public abstract Color TextMuted     { get; }
    public abstract Color Error         { get; }
    public abstract Color Warn          { get; }
    public abstract Color StatusOn      { get; }
    public abstract Color StatusOnRing  { get; }
    public abstract Color StatusOff     { get; }
    public abstract Color TextPrint     { get; }
    public abstract Color TextInfo      { get; }
    public abstract Color TextWarn      { get; }
    public abstract Color TextError     { get; }
    public abstract Color GlowPrimary        { get; }
    public abstract double GlowPrimaryBlur   { get; }
    public abstract double GlowPrimaryOpacity { get; }
    public abstract Color GlowLogo           { get; }
    public abstract double GlowLogoBlur      { get; }
    public abstract Color WinClose    { get; }
    public abstract Color WinMinimize { get; }
    public abstract Color WinMaximize { get; }
    public abstract Color Shadow      { get; }
    public abstract string EditorFontFamily  { get; }
    public abstract double EditorFontSize    { get; }
    public abstract double EditorLineHeight  { get; }

    protected static Color Hex(string hex)
    {
        hex = hex.TrimStart('#');
        byte r = Convert.ToByte(hex[0..2], 16);
        byte g = Convert.ToByte(hex[2..4], 16);
        byte b = Convert.ToByte(hex[4..6], 16);
        return Color.FromRgb(r, g, b);
    }
}
