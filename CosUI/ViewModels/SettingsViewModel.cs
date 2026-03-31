using CosUI.Themes;

namespace CosUI.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private int _maxFps = 60;
    public int MaxFps
    {
        get => _maxFps;
        set { Set(ref _maxFps, value); Cosmic.SetMaxFps(value); }
    }

    private bool _unlockFps;
    public bool UnlockFps
    {
        get => _unlockFps;
        set { Set(ref _unlockFps, value); Cosmic.SetUnlockFps(value); }
    }

    private bool _autoExec;
    public bool AutoExec
    {
        get => _autoExec;
        set { Set(ref _autoExec, value); Cosmic.SetAutoExecute(value); }
    }

    private bool _consoleRedirect = true;
    public bool ConsoleRedirect
    {
        get => _consoleRedirect;
        set { Set(ref _consoleRedirect, value); Cosmic.SetConsoleRedirect(value); }
    }

    private string _consoleFilter = "MESSAGE_OUTPUT";
    public string ConsoleFilter
    {
        get => _consoleFilter;
        set { Set(ref _consoleFilter, value); Cosmic.SetConsoleFilter(value); }
    }

    public string[] FilterOptions { get; } =
        ["MESSAGE_NONE", "MESSAGE_OUTPUT", "MESSAGE_WARN", "MESSAGE_ERROR"];

    public ITheme[] ThemeOptions { get; } =
    [
        new BlueVoidTheme(),
        new GreenVoidTheme(),
        new PurpleVoidTheme(),
        new RedVoidTheme(),
        new ObsidianTheme(),
        new DarkTheme(),
        new LightTheme(),
    ];

    public ITheme SelectedTheme
    {
        get => ThemeOptions.FirstOrDefault(t => t.Name == App.Theme.Current.Name) ?? ThemeOptions[0];
        set
        {
            if (value is null || value.Name == App.Theme.Current.Name) return;
            App.Theme.Apply(value);
            OnPropertyChanged();
        }
    }
}
