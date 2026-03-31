using System.IO;
using System.Windows;
using CosUI.Services;
using CosUI.ViewModels;
using CosUI.Views;

namespace CosUI;

public partial class App : Application
{
    private static readonly string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private static readonly string DataDir = Path.Combine(LocalAppData, "CosUI");
    public static readonly string ScriptsDir = Path.Combine(DataDir, "Scripts");
    public static readonly string AutoExecDir = @"C:\Cosmic\AutoExec";
    public static readonly string SessionPath = Path.Combine(DataDir, "session.json");

    public static ThemeService Theme { get; } = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DispatcherUnhandledException += (_, args) => { args.Handled = true; };

        try
        {
            Theme.Apply(Theme.Current);
            _ = Cosmic.Initialize();
            ShowWelcome();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    private void ShowWelcome()
    {
        var session = new SessionService(SessionPath);
        var files = new FileService(ScriptsDir, AutoExecDir);
        var vm = new WelcomeViewModel(files, session) { ServerRunning = Cosmic.IsRunning };
        var welcome = new WelcomeWindow(vm);
        welcome.ActionSelected += (path, restore) =>
        {
            var main = new MainWindow(path, restore);
            main.Show();
        };
        welcome.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Cosmic.Shutdown();
        base.OnExit(e);
    }
}
