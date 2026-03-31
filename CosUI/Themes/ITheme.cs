using System.Windows.Media;

namespace CosUI.Themes;

public interface ITheme
{
    string Name { get; }

    Color Background    { get; }
    Color Surface       { get; }
    Color SidebarBg     { get; }
    Color BorderSubtle  { get; }
    Color BorderActive  { get; }
    Color PrimaryBlue   { get; }
    Color AccentBlue    { get; }
    Color TextPrimary   { get; }
    Color TextSecondary { get; }
    Color TextMuted     { get; }
    Color Error         { get; }
    Color Warn          { get; }
    Color StatusOn      { get; }
    Color StatusOnRing  { get; }
    Color StatusOff     { get; }
    Color TextPrint     { get; }
    Color TextInfo      { get; }
    Color TextWarn      { get; }
    Color TextError     { get; }

    Color GlowPrimary        { get; }
    double GlowPrimaryBlur   { get; }
    double GlowPrimaryOpacity { get; }
    Color GlowLogo           { get; }
    double GlowLogoBlur      { get; }

    Color WinClose    { get; }
    Color WinMinimize { get; }
    Color WinMaximize { get; }
    Color Shadow      { get; }

    string EditorFontFamily  { get; }
    double EditorFontSize    { get; }
    double EditorLineHeight  { get; }
}
