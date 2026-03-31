using System.Windows;
using System.Windows.Input;
using CosUI.Themes;
using CosUI.ViewModels;
using Microsoft.Win32;

namespace CosUI.Views;

public partial class WelcomeWindow : Window
{
    private readonly WelcomeViewModel _vm;

    public WelcomeWindow(WelcomeViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;
        UpdateStatusDisplay();
        vm.PropertyChanged += (_, _) => UpdateStatusDisplay();
    }

    private void UpdateStatusDisplay()
    {
        WsStatusBadge.Text = _vm.ServerRunning ? "Active" : "Offline";
        WsStatusBadge.Foreground = _vm.ServerRunning
            ? (System.Windows.Media.Brush)FindResource(ThemeKeys.Brush(ThemeKeys.PrimaryBlue))
            : (System.Windows.Media.Brush)FindResource(ThemeKeys.Brush(ThemeKeys.TextMuted));
        RobloxStatusBadge.Text = _vm.OpenScriptCount > 0 ? $"{_vm.OpenScriptCount} attached" : "Not attached";
        OpenScriptCountText.Text = _vm.OpenScriptCount.ToString();
        AutoExecBadge.Text = _vm.AutoExecOn ? "On" : "Off";
        AutoExecBadge.Foreground = _vm.AutoExecOn
            ? (System.Windows.Media.Brush)FindResource(ThemeKeys.Brush(ThemeKeys.PrimaryBlue))
            : (System.Windows.Media.Brush)FindResource(ThemeKeys.Brush(ThemeKeys.TextMuted));
        LiveDot.IsActive = _vm.ServerRunning;
    }

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }

    /// <summary>Fired when user picks an action. Args: (path, restore). path=null for new file or restore.</summary>
    public event Action<string?, bool>? ActionSelected;

    private void Continue_Click(object sender, MouseButtonEventArgs e)
    {
        ActionSelected?.Invoke(null, true);
        Close();
    }

    private void NewScript_Click(object sender, MouseButtonEventArgs e)
    {
        ActionSelected?.Invoke(null, false);
        Close();
    }

    private void OpenFile_Click(object sender, MouseButtonEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "Luau files (*.luau;*.lua)|*.luau;*.lua|All files (*.*)|*.*"
        };
        if (dlg.ShowDialog() == true)
            ActionSelected?.Invoke(dlg.FileName, false);
        else
            ActionSelected?.Invoke(null, false);
        Close();
    }

    private void Settings_Click(object sender, MouseButtonEventArgs e)
    {
        new SettingsOverlay { Owner = this }.ShowDialog();
    }

    private void ClearHistory_Click(object sender, MouseButtonEventArgs e)
    {
        _vm.ClearHistory();
    }

    private void RecentItem_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.Tag is RecentEntry entry)
        {
            ActionSelected?.Invoke(entry.Path, false);
            Close();
        }
    }
}
