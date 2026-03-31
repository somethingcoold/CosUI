using System.Windows;
using System.Windows.Media;
using CosUI.Themes;

namespace CosUI.Services;

public class ThemeService
{
    public ITheme Current { get; private set; } = new BlueVoidTheme();

    public event Action<ITheme>? ThemeChanged;

    public ResourceDictionary BuildDictionary(ITheme theme)
    {
        var dict = new ResourceDictionary();
        Add(dict, ThemeKeys.Background,    theme.Background);
        Add(dict, ThemeKeys.Surface,       theme.Surface);
        Add(dict, ThemeKeys.SidebarBg,     theme.SidebarBg);
        Add(dict, ThemeKeys.BorderSubtle,  theme.BorderSubtle);
        Add(dict, ThemeKeys.BorderActive,  theme.BorderActive);
        Add(dict, ThemeKeys.PrimaryBlue,   theme.PrimaryBlue);
        Add(dict, ThemeKeys.AccentBlue,    theme.AccentBlue);
        Add(dict, ThemeKeys.TextPrimary,   theme.TextPrimary);
        Add(dict, ThemeKeys.TextSecondary, theme.TextSecondary);
        Add(dict, ThemeKeys.TextMuted,     theme.TextMuted);
        Add(dict, ThemeKeys.Error,         theme.Error);
        Add(dict, ThemeKeys.Warn,          theme.Warn);
        Add(dict, ThemeKeys.StatusOn,      theme.StatusOn);
        Add(dict, ThemeKeys.StatusOnRing,  theme.StatusOnRing);
        Add(dict, ThemeKeys.StatusOff,     theme.StatusOff);
        Add(dict, ThemeKeys.TextPrint,     theme.TextPrint);
        Add(dict, ThemeKeys.TextInfo,      theme.TextInfo);
        Add(dict, ThemeKeys.TextWarn,      theme.TextWarn);
        Add(dict, ThemeKeys.TextError,     theme.TextError);
        Add(dict, ThemeKeys.GlowPrimary,   theme.GlowPrimary);
        Add(dict, ThemeKeys.GlowLogo,      theme.GlowLogo);
        Add(dict, ThemeKeys.WinClose,      theme.WinClose);
        Add(dict, ThemeKeys.WinMinimize,   theme.WinMinimize);
        Add(dict, ThemeKeys.WinMaximize,   theme.WinMaximize);
        Add(dict, ThemeKeys.Shadow,        theme.Shadow);
        return dict;
    }

    public void Apply(ITheme theme)
    {
        Current = theme;
        var dict = BuildDictionary(theme);
        var merged = Application.Current.Resources.MergedDictionaries;
        if (merged.Count == 0) merged.Add(dict);
        else merged[0] = dict;
        ThemeChanged?.Invoke(theme);
    }

    private static void Add(ResourceDictionary dict, string key, Color color)
    {
        dict[key] = color;
        dict[ThemeKeys.Brush(key)] = new SolidColorBrush(color);
    }
}
